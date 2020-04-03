using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class Window_Graph : MonoBehaviour
{
    //intialize things

    public Text startRange;
    public Text endRange;
    public Text date;
    public GameObject SearchPanel;
    public GameObject DateRangePanel;
    public Text DisplayPanel;

    private static Window_Graph instance;

    [SerializeField] private Sprite dotSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;
    private List<IGraphVisualObject> graphVisualObjectList;
    private GameObject tooltipGameObject;
    private List<RectTransform> yLabelList;

    //cached values
    private List<int> valueList;
    private IGraphVisual graphVisual;
    private int maxVisibleValueAmount;
    private Func<int, string> getAxisLabelX;
    private Func<float, string> getAxisLabelY;    
    private float xSize;
    private bool startYScaleAtZero;    

    IEnumerator Start()
    {
                
        instance = this;
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        tooltipGameObject = graphContainer.Find("tooltip").gameObject;

        startYScaleAtZero = true;
        gameObjectList = new List<GameObject>();
        yLabelList = new List<RectTransform>();
        graphVisualObjectList = new List<IGraphVisualObject>();            

        IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, new Color32(85, 165, 255, 255), new Color32(85, 165, 255, 100));
        IGraphVisual barChartVisual = new BarChartVisual(graphContainer, new Color32(85, 165, 255, 255), .8f);

        //buttons to change graph interface
        transform.Find("barChartBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            SetGraphVisual(barChartVisual);
        };
        transform.Find("lineGraphBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            SetGraphVisual(lineGraphVisual);
        };
        transform.Find("decreaseVisibleAmountBtn").GetComponent<Button_UI>().ClickFunc = () => {
            DecreaseVisibleAmount();
        };
        transform.Find("increaseVisibleAmountBtn").GetComponent<Button_UI>().ClickFunc = () => {
            IncreaseVisibleAmount();
        };
        

        //values to be used in the graph
        List<int> valueList = new List<int>() {};
        WWW tagsTime = new WWW("http://localhost:81/percentsdata/showTotalTime.php");
        yield return tagsTime;
        string[] timeArray = tagsTime.text.Split(';');

        int numrows = int.Parse(timeArray[0]);

        bool skipFirst = false;
        foreach (string str in timeArray)
        {
            if (skipFirst)
            {
                int mins = (Int32.Parse(str)) / 60 / 60;
                valueList.Add(mins);
            }
            skipFirst = true;
        }

        WWW tagsName = new WWW("http://localhost:81/percentsdata/ShowTags.php");
        yield return tagsName;
        //string tagsDataString = tagsData.text;
        string[] nameArray = tagsName.text.Split(';');
        int num = Int32.Parse(nameArray[0]);
        string[] names = new string[num];
        Array.Copy(nameArray, 1, names, 0, num);        

        //to be performed on startup
        ShowGraph(valueList, barChartVisual, -1, (int _i) => names[(_i )] , (float _f) => Mathf.RoundToInt(_f) + " hrs");     
    }

    IEnumerator ChangeData()
    {
        IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, new Color32(85, 165, 255, 255), new Color32(85, 165, 255, 100));
        IGraphVisual barChartVisual = new BarChartVisual(graphContainer, new Color32(85, 165, 255, 255), .8f);       
       
        WWWForm form = new WWWForm();
                
        string starttime = startRange.text;
        string endtime = endRange.text;
       
        form.AddField("starttime", starttime);
        form.AddField("endtime", endtime);
        
        WWW getNewTimes = new WWW("http://localhost:81/percentsdata/getNewTimes.php", form);
        yield return getNewTimes;        

        string[] newtimes = getNewTimes.text.Split(';');        
        List<int> newList = new List<int>();

        int numrows = int.Parse(newtimes[0]);

        bool skipFirst = false;
        foreach (string str in newtimes)
        {
            
            if (skipFirst)
            {
                int mins = (Int32.Parse(str)) / 60 / 60;
                newList.Add(mins);
            }
            skipFirst = true;            
        }
        
        WWW getNewTags = new WWW("http://localhost:81/percentsdata/getNewTags.php", form);
       
        yield return getNewTags;
        //string tagsDataString = tagsData.text;
        string[] nameArray = getNewTags.text.Split(';');
        int num = Int32.Parse(nameArray[0]);
        string[] names = new string[num];
        Array.Copy(nameArray, 1, names, 0, num);

        HideSearchPanel();
        ShowGraph(newList, barChartVisual, -1, (int _i) => names[(_i)], (float _f) => Mathf.RoundToInt(_f) + " hrs");

        //Debug.Log("text!!" + starttime);
        string DisplayInfo = starttime + " to " + endtime;
        UpdateDisplayPanel(DisplayInfo);
    }

    public void CallChangeData()
    {
        StartCoroutine(ChangeData());
    }

    IEnumerator NewDateData()
    {
        IGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, new Color32(85, 165, 255, 255), new Color32(85, 165, 255, 100));
        IGraphVisual barChartVisual = new BarChartVisual(graphContainer, new Color32(85, 165, 255, 255), .8f);

        WWWForm form = new WWWForm();

        string newDate = date.text;

        string NewYearTimes = "http://localhost:81/percentsdata/NewYearTimes.php";
        string NewMonthTimes = "http://localhost:81/percentsdata/NewMonthTimes.php";
        string NewDayTimes = "http://localhost:81/percentsdata/NewDayTimes.php";

        string NewYearTags = "http://localhost:81/percentsdata/NewYearTags.php";
        string NewMonthTags = "http://localhost:81/percentsdata/NewMonthTags.php";
        string NewDayTags = "http://localhost:81/percentsdata/NewDayTags.php";

        string toUseTime ="";
        string toUseTag ="";

        if (newDate.Length == 4)
        {
            toUseTime = NewYearTimes;
            toUseTag = NewYearTags;
        }
        else if (newDate.Length == 7)
        {
            toUseTime = NewMonthTimes;
            toUseTag = NewMonthTags;
        }
        else
        {
            toUseTime = NewDayTimes;
            toUseTag = NewDayTags;
        }

        form.AddField("date", newDate);
        WWW getNewTimes = new WWW(toUseTime, form);
        yield return getNewTimes;


        string[] newtimes = getNewTimes.text.Split(';');
        List<int> newList = new List<int>();

        int numrows = int.Parse(newtimes[0]);

        bool skipFirst = false;
        foreach (string str in newtimes)
        {
            if (skipFirst)
            {
                int mins = (Int32.Parse(str)) / 60 / 60;
                newList.Add(mins);
            }
            skipFirst = true;
        }

        
        WWW getNewTags = new WWW(toUseTag, form);
        yield return getNewTags;
        
        string[] nameArray = getNewTags.text.Split(';');
        int num = Int32.Parse(nameArray[0]);
        string[] names = new string[num];
        Array.Copy(nameArray, 1, names, 0, num);

        HideSearchPanel();
        ShowGraph(newList, barChartVisual, -1, (int _i) => names[(_i)], (float _f) => Mathf.RoundToInt(_f) + " hrs");        
        UpdateDisplayPanel(date.text);
    }

    public void CallNewDateData()
    {
        StartCoroutine(NewDateData());
        HideSearchPanel();
    }

    public void UpdateDisplayPanel(string str)
    {
        DisplayPanel.text=str;
    }

    public void ShowSearchPanel()
    {
        SearchPanel.SetActive(true);
    }

    public void HideSearchPanel()
    {
        SearchPanel.SetActive(false);
    }

    public void ShowDateRangePanel()
    {
        DateRangePanel.SetActive(true);
    }

    public void HideDateRangePanel()
    {
        DateRangePanel.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipText, Vector2 anchoredPosition)
    {
        anchoredPosition.x -= 450f;
        anchoredPosition.y -= 200f;
        instance.ShowTooltip(tooltipText, anchoredPosition);
    }
    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }

    private void ShowTooltip(string tooltipText, Vector2 anchoredPosition)
    {
        tooltipGameObject.SetActive(true);

        tooltipGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        Text tooltipUIText = tooltipGameObject.transform.Find("text").GetComponent<Text>();
        tooltipUIText.text = tooltipText;

        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(
            tooltipUIText.preferredWidth + textPaddingSize * 2f,
            tooltipUIText.preferredHeight + textPaddingSize * 2f
            );

        tooltipGameObject.transform.Find("background").GetComponent<RectTransform>().sizeDelta = backgroundSize;

        tooltipGameObject.transform.SetAsLastSibling();
    }

    private void HideTooltip()
    {
        tooltipGameObject.SetActive(false);
    }

    private void SetGetAxisLabelX(Func<int, string> getAxisLabelX)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, getAxisLabelX, this.getAxisLabelY);
    }

    private void SetGetAxisLabelY(Func<float, string> getAxisLabelY)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, getAxisLabelY);
    }

    private void IncreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLabelX, this.getAxisLabelY);
    }
    private void DecreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount - 1, this.getAxisLabelX, this.getAxisLabelY);
    }

    private void SetGraphVisual(IGraphVisual graphVisual)
    {
        ShowGraph(this.valueList, graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, this.getAxisLabelY);
    }

    //place points on graph in accordance to data from the valueList
    private void ShowGraph(List<int> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null)
    {
        this.valueList = valueList;
        this.graphVisual = graphVisual;
        this.getAxisLabelX = getAxisLabelX;
        this.getAxisLabelY = getAxisLabelY;

        
        if (maxVisibleValueAmount <= 0) //variable used to determine how many points to show on the x axis
        {
            maxVisibleValueAmount = valueList.Count;
        }
        if (maxVisibleValueAmount > valueList.Count) //variable used to determine how many points to show on the x axis
        {
            maxVisibleValueAmount = valueList.Count;
        }
        this.maxVisibleValueAmount = maxVisibleValueAmount;

        //determines what the x and y labels will be when not given an argument
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        //clean up previous graph
        foreach (GameObject gameObject in gameObjectList) //destroy all previously made game objects before spawning new ones
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();
        yLabelList.Clear();

        foreach (IGraphVisualObject graphVisualObject in graphVisualObjectList)
        {
            graphVisualObject.CleanUp();
        }
        graphVisualObjectList.Clear();
        graphVisual.CleanUp();

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMinimum, yMaximum;
        CalculateScale(out yMinimum, out yMaximum);


        xSize = graphWidth / (maxVisibleValueAmount + 1); //adjust spacing of data points in accordance to the graph width and the maxVisibleValueAmount, with some extra room at the end

        int xIndex = 0; //variable used to determine how many points to show on the x axis
        
        //GameObject lastDotGameObject = null; //creates a reference to the previously created dot, to be used in connecting the dots in CreateDotConnection
        //for loop to cycle through the data in the valueList, and create the data points, x labels, etc...
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            float xPosition = xSize + xIndex * xSize;    //determines the horizontal spacing between points
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight; //determines the vertical spacing between points

            string tooltipText = getAxisLabelY(valueList[i]);
            graphVisualObjectList.Add(graphVisual.CreateGraphVisualObject(new Vector2(xPosition, yPosition), xSize, tooltipText));

            //create the x label
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -7f); //xlabel's x position is same as dot, y is under the graph
            labelX.GetComponent<Text>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject); //add this gameobject to the list to be destroyed after spawning

            //create the dashes for the x labels
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -3f);
            gameObjectList.Add(dashX.gameObject); //add this gameobject to the list to be destroyed after spawning

            xIndex++;
        }
        //create the y label
        int seperatorCount = 10;
        for (int i = 0; i <= seperatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / seperatorCount; //determines y labels position in relation to the seperatorCount
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight); //uses the above normalizedValue and fits it in with the graphHeight
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum))); //determine what the y label will display
            yLabelList.Add(labelY);
            gameObjectList.Add(labelY.gameObject); //add this gameobject to the list to be destroyed after spawning

            //create the dashes for the y labels
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject); //add this gameobject to the list to be destroyed after spawning

        }
    }

    private void UpdateValue(int index, int value)
    {
        float yMinimumBefore, yMaximumBefore;
        CalculateScale(out yMinimumBefore, out yMaximumBefore);

        valueList[index] = value;

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMinimum, yMaximum;
        CalculateScale(out yMinimum, out yMaximum);

        bool yScaleChanged = yMinimumBefore != yMinimum || yMaximumBefore != yMaximum;

        if (!yScaleChanged)
        {
            float xPosition = xSize + index * xSize;
            float yPosition = ((value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            //Add data point visual
            string tooltipText = getAxisLabelY(value);
            graphVisualObjectList[index].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, tooltipText);
        }
        else
        {
            int xIndex = 0; //variable used to determine how many points to show on the x axis        
            for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
            {
                float xPosition = xSize + xIndex * xSize;    //determines the horizontal spacing between points
                float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight; //determines the vertical spacing between points

                string tooltipText = getAxisLabelY(valueList[i]);
                graphVisualObjectList[xIndex].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), xSize, tooltipText);

                xIndex++;
            }

            for (int i = 0; i < yLabelList.Count; i++)
            {
                float normalizedValue = i * 1f / yLabelList.Count;
                yLabelList[i].GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            }
        }        
    }

    private void CalculateScale(out float yMinimum, out float yMaximum)
    {
        yMaximum = valueList[0];
        yMinimum = valueList[0];

        //determines max and min value in valueList, to be used in deciding the highest and lowest y readings of the graph
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            int value = valueList[i];
            if (value > yMaximum)
            {
                yMaximum = value;
            }
            if (value < yMinimum)
            {
                yMinimum = value;
            }
        }

        float yDifference = yMaximum - yMinimum; //to be used when showing one value int the graph, to correctly control the upper buffer
        if (yDifference <= 0)
        {
            yDifference = 5f;
        }
        yMaximum = yMaximum + (yDifference * 0.2f);   //increase the ymaximum a bit, so the highest value isn't at the very top of the graph
        yMinimum = yMinimum - (yDifference * 0.2f); //same as above, but for the bottom of the graph

        if (startYScaleAtZero)
        {
            yMinimum = 0f; //a new x index to be used in the below for loop, since i is no longer starting from 0;
        }
    }

    private interface IGraphVisual
    {
        IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        void CleanUp();
    }

    //Represents a single Visual Object in the graph
    private interface IGraphVisualObject
    {
        void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        void CleanUp();
    }

    //class to create the bar chart visual construct, which has all code in regards to the bar chart
    private class BarChartVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Color barColor;
        private float barWidthMultiplier;

        public BarChartVisual(RectTransform graphContainer, Color barColor, float barWidthMultiplier)
        {
            this.graphContainer = graphContainer;
            this.barColor = barColor;
            this.barWidthMultiplier = barWidthMultiplier;
        }

        public void CleanUp() { }

        public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth);

            BarChartVisualObject barChartVisualObject = new BarChartVisualObject(barGameObject, barWidthMultiplier);
            barChartVisualObject.SetGraphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);


            return barChartVisualObject;
            //because the lineGraphVisual returns two gameobjects, the dot itself and the dot connection, a list of gameobjects is used. To keep the arguments the same between 
            //barGraphVisual and lineGraphVisual, we use a list of game objects for barGraphVisual as well, even though we're only returning one game object
        }

        private GameObject CreateBar(Vector2 graphPosition, float barWidth)
        {
            GameObject gameObject = new GameObject("bar", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = barColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
            rectTransform.sizeDelta = new Vector2(barWidth * barWidthMultiplier, graphPosition.y);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(.5f, 0f);

            Button_UI barButtonUI = gameObject.AddComponent<Button_UI>();

            return gameObject;
        }

        public class BarChartVisualObject : IGraphVisualObject
        {
            private GameObject barGameObject;
            private float barWidthMultiplier;

            public BarChartVisualObject(GameObject barGameObject, float barWidthMultiplier)
            {
                this.barGameObject = barGameObject;
                this.barWidthMultiplier = barWidthMultiplier;
            }

            public void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
            {
                RectTransform rectTransform = barGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
                rectTransform.sizeDelta = new Vector2(graphPositionWidth * barWidthMultiplier, graphPosition.y);

                Button_UI barButtonUI = barGameObject.GetComponent<Button_UI>();

                //Show Tooltip on mouse over
                barButtonUI.MouseOverOnceFunc = () =>
                {
                    ShowTooltip_Static(tooltipText, graphPosition);
                };

                //Hide Tooltip on mouse out
                barButtonUI.MouseOutOnceFunc = () =>
                {
                    HideTooltip_Static();
                };
            }

            public void CleanUp()
            {
                Destroy(barGameObject);
            }
        }
    }

    private class LineGraphVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Sprite dotSprite;
        private LineGraphVisualObject lastLineGraphVisualObject;
        private Color dotColor;
        private Color dotConnectionColor;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;
            this.dotSprite = dotSprite;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
            lastLineGraphVisualObject = null;
        }

        public void CleanUp()
        {
            lastLineGraphVisualObject = null;
        }

        public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            //List<GameObject> gameObjectList = new List<GameObject>();
            GameObject dotGameObject = CreateDot(graphPosition); //create a circle on the above x and y points
            //gameObjectList.Add(dotGameObject); //add this gameobject to the list to be destroyed after spawning

            GameObject dotConnectionGameObject = null;
            if (lastLineGraphVisualObject != null)
            {
                //if not null(if not the first point), make a line between it and the previous dot
                dotConnectionGameObject = CreateDotConnection(lastLineGraphVisualObject.GetGraphPosition(), dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                // gameObjectList.Add(dotConnectionGameObject); //add this gameobject to the list to be destroyed after spawning
            }

            LineGraphVisualObject lineGraphVisualObject = new LineGraphVisualObject(dotGameObject, dotConnectionGameObject, lastLineGraphVisualObject);
            lineGraphVisualObject.SetGraphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);

            lastLineGraphVisualObject = lineGraphVisualObject;
            return lineGraphVisualObject;
        }

        //function to create dot sprite for data points
        private GameObject CreateDot(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("dot", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().sprite = dotSprite;
            gameObject.GetComponent<Image>().color = dotColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);

            Button_UI dotButtonUI = gameObject.AddComponent<Button_UI>();

            return gameObject;
        }

        //function to create lines connecting the dots
        private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor; //make lines between equal to the dotConnectionColor
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 dir = (dotPositionB - dotPositionA).normalized; //stores the direction for the line to face from dot a to b. And then normalizes it?
            float distance = Vector2.Distance(dotPositionA, dotPositionB); //determines distance between points a and b, to be used for calculating the length of the connecting line
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f); //set length of line equal to distance between dots
            rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f; //anchor line's position to halfway between dots a and b
            rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir)); //not too sure, but seems to calulate an angle from 0 to 360 degrees based on a vector2
            return gameObject;
        }

        public class LineGraphVisualObject : IGraphVisualObject
        {
            public event EventHandler OnChangedGraphVisualObjectInfo;

            private GameObject dotGameObject;
            private GameObject dotConnectionGameObject;
            private LineGraphVisualObject lastVisualObject;

            public LineGraphVisualObject(GameObject dotGameObject, GameObject dotConnectionGameObject, LineGraphVisualObject lastVisualObject)
            {
                this.dotGameObject = dotGameObject;
                this.dotConnectionGameObject = dotConnectionGameObject;
                this.lastVisualObject = lastVisualObject;

                if (lastVisualObject != null)
                {
                    lastVisualObject.OnChangedGraphVisualObjectInfo += LastVisualObject_OnChangedGraphVisualObjectInfo;
                }
            }

            private void LastVisualObject_OnChangedGraphVisualObjectInfo(object sender, EventArgs e)
            {
                UpdateDotConnection();
            }

            public void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = graphPosition;

                UpdateDotConnection();

                Button_UI dotButtonUI = dotGameObject.GetComponent<Button_UI>();

                dotButtonUI.MouseOverOnceFunc = () =>
                {
                    ShowTooltip_Static(tooltipText, graphPosition);
                };

                dotButtonUI.MouseOutOnceFunc = () =>
                {
                    HideTooltip_Static();
                };

                if (OnChangedGraphVisualObjectInfo != null) OnChangedGraphVisualObjectInfo(this, EventArgs.Empty);
            }

            public void CleanUp()
            {
                Destroy(dotGameObject);
                Destroy(dotConnectionGameObject);
            }

            public Vector2 GetGraphPosition()
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                return rectTransform.anchoredPosition;
            }

            private void UpdateDotConnection()
            {
                if (dotConnectionGameObject != null)
                {
                    RectTransform dotConnectionRectTransform = dotConnectionGameObject.GetComponent<RectTransform>();
                    Vector2 dir = (lastVisualObject.GetGraphPosition() - GetGraphPosition()).normalized; //stores the direction for the line to face from dot a to b. And then normalizes it?
                    float distance = Vector2.Distance(GetGraphPosition(), lastVisualObject.GetGraphPosition()); //determines distance between points a and b, to be used for calculating the length of the connecting line

                    dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3f); //set length of line equal to distance between dots
                    dotConnectionRectTransform.anchoredPosition = GetGraphPosition() + dir * distance * .5f; //anchor line's position to halfway between dots a and b
                    dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir)); //not too sure, but seems to calulate an angle from 0 to 360 degrees based on a vector2                    
                }
            }
        }
    }
}
