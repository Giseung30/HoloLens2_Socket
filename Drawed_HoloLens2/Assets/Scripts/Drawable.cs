using UnityEngine;

public class Drawable : MonoBehaviour
{
    [Header("Scripts")]
    public static Drawable drawable;

    [Header("Option")]
    public Color penColour = Color.red; //�� ����
    public int penWidth = 3; //���� �β�
    public LayerMask drawingLayers; //�׸� �� �ִ� ���̾�
    public Sprite drawableSprite; //��ǥ Sprite

    [Header("Cache")]
    private Texture2D drawableTexture; //Sprite�κ��� ����� Texture2D ����
    private Vector2 previousDragPosition; //���� �巡�� ��ġ
    private Color[] cleanColoursArray; //�ʱ�ȭ ���� �迭
    private Color32[] curColors; //Texture ����
    private bool mouseWasPreviouslyHeldDown = false; //���� ���콺 ��ġ ����
    private bool noDrawingOnCurrentDrag = false;
    public Vector2 mouseWorldPosition; //���콺 ���� ��ġ
    public bool clearButtonMouseDown; //Canvas �ʱ�ȭ ��ư ���콺 ȣ���� ����

    private void Awake()
    {
        drawable = this;

        drawableTexture = drawableSprite.texture; //Sprite�κ��� Texture ���� ����

        /* �ʱ�ȭ ���� ���� */
        cleanColoursArray = new Color[(int)drawableSprite.rect.width * (int)drawableSprite.rect.height];
        for (int i = 0; i < cleanColoursArray.Length; ++i)
            cleanColoursArray[i] = new Color(1f, 1f, 1f, 0f);

        ClearCanvas(); //Canvas �ʱ�ȭ
    }

    private void Update()
    {
        bool mouseHeldDown = Client.client.mReceivePacket.mouseHeldDown; //���콺 ȣ���� ���� ����

        if (mouseHeldDown && !noDrawingOnCurrentDrag)
        {
            mouseWorldPosition = new Vector2(Client.client.mReceivePacket.mousePositionX, Client.client.mReceivePacket.mousePositionY); //���콺 ��ġ ����

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPosition, drawingLayers.value);
            if (hit != null && hit.transform != null)
            {
                PenBrush(mouseWorldPosition);
            }

            else
            {
                previousDragPosition = Vector2.zero;

                if (!mouseWasPreviouslyHeldDown)
                {
                    noDrawingOnCurrentDrag = true;
                }
            }
        }

        else if (!mouseHeldDown)
        {
            previousDragPosition = Vector2.zero;
            noDrawingOnCurrentDrag = false;
        }
        mouseWasPreviouslyHeldDown = mouseHeldDown;

        if (Client.client.mReceivePacket.clearButtonMouseDown) //Canvas �ʱ�ȭ ��ư�� ȣ���� ���̸�
        {
            ClearCanvas(); //Canvas �ʱ�ȭ
        }

        penColour = new Color(Client.client.mReceivePacket.penColourR, Client.client.mReceivePacket.penColourG, Client.client.mReceivePacket.penColourB); //�� ���� ����
        penWidth = Client.client.mReceivePacket.penWidth; //�� �β� ����
    }

    public void BrushTemplate(Vector2 worldPosition)
    {
        Vector2 pixelPos = WorldToPixelCoordinates(worldPosition);

        curColors = drawableTexture.GetPixels32();

        if (previousDragPosition == Vector2.zero)
        {
            MarkPixelsToColour(pixelPos, penWidth, penColour);
        }
        else
        {
            ColourBetween(previousDragPosition, pixelPos, penWidth, penColour);
        }
        ApplyMarkedPixelChanges();

        previousDragPosition = pixelPos;
    }

    public void PenBrush(Vector2 worldPoint)
    {
        Vector2 pixelPos = WorldToPixelCoordinates(worldPoint);

        curColors = drawableTexture.GetPixels32();

        if (previousDragPosition == Vector2.zero)
        {
            MarkPixelsToColour(pixelPos, penWidth, penColour);
        }
        else
        {
            ColourBetween(previousDragPosition, pixelPos, penWidth, penColour);
        }
        ApplyMarkedPixelChanges();

        previousDragPosition = pixelPos;
    }

    public void ColourBetween(Vector2 startPoint, Vector2 endPoint, int width, Color color)
    {
        float distance = Vector2.Distance(startPoint, endPoint);

        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            Vector2 curPosition = Vector2.Lerp(startPoint, endPoint, lerp);
            MarkPixelsToColour(curPosition, width, color);
        }
    }

    public void MarkPixelsToColour(Vector2 centerPixel, int penThickness, Color colorOfPen)
    {
        int centerX = (int)centerPixel.x;
        int centerY = (int)centerPixel.y;

        for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
        {
            if (x >= (int)drawableSprite.rect.width || x < 0)
                continue;

            for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
            {
                MarkPixelToChange(x, y, colorOfPen);
            }
        }
    }

    public void MarkPixelToChange(int x, int y, Color color)
    {
        int arrayPos = y * (int)drawableSprite.rect.width + x;

        if (arrayPos > curColors.Length || arrayPos < 0)
            return;

        curColors[arrayPos] = color;
    }

    public void ApplyMarkedPixelChanges()
    {
        drawableTexture.SetPixels32(curColors);
        drawableTexture.Apply();
    }

    public void ColourPixels(Vector2 centerPixel, int penThickness, Color colorOfPen)
    {
        int centerX = (int)centerPixel.x;
        int centerY = (int)centerPixel.y;

        for (int x = centerX - penThickness; x <= centerX + penThickness; x++)
        {
            for (int y = centerY - penThickness; y <= centerY + penThickness; y++)
            {
                drawableTexture.SetPixel(x, y, colorOfPen);
            }
        }

        drawableTexture.Apply();
    }

    public Vector2 WorldToPixelCoordinates(Vector2 worldPosition)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);

        float pixelWidth = drawableSprite.rect.width;
        float pixelHeight = drawableSprite.rect.height;
        float unitsToPixels = pixelWidth / drawableSprite.bounds.size.x * transform.localScale.x;

        float centeredX = localPos.x * unitsToPixels + pixelWidth / 2;
        float centeredY = localPos.y * unitsToPixels + pixelHeight / 2;

        Vector2 pixelPos = new Vector2(Mathf.RoundToInt(centeredX), Mathf.RoundToInt(centeredY));

        return pixelPos;
    }

    /* Canvas �ʱ�ȭ�ϴ� �Լ� */
    public void ClearCanvas()
    {
        drawableTexture.SetPixels(cleanColoursArray);
        drawableTexture.Apply();
    }

    /* Canvas �ʱ�ȭ ��ư ȣ���� ���θ� �����ϴ� �Լ� */
    public void OnMouseClearButton(bool mouseDown)
    {
        clearButtonMouseDown = mouseDown;
    }

    /* �� ������ �����ϴ� �Լ� */
    public void SetMarkerColour(string color)
    {
        switch (color)
        {
            case "red":
                penColour = new Color(1f, 0f, 0f, 1f);
                break;
            case "green":
                penColour = new Color(0f, 1f, 0f, 1f);
                break;
            case "blue":
                penColour = new Color(0f, 0f, 1f, 1f);
                break;
            case "yellow":
                penColour = new Color(1f, 1f, 0f, 1f);
                break;
        }

    }

    /* �� �β� �����ϴ� �Լ� */
    public void SetMarkerWidth(int newWidth)
    {
        penWidth = newWidth;
    }

    /* �� �β� �����ϴ� �Լ� - �����̴��� */
    public void SetMarkerWidth(float newWidth)
    {
        SetMarkerWidth((int)newWidth);
    }
}