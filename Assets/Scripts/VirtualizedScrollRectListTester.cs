// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using MixedReality.Toolkit.UX;
using MixedReality.Toolkit.UX.Experimental;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("MRTK/Examples/Virtualized Scroll Rect List Tester")]
    public class VirtualizedScrollRectListTester : MonoBehaviour
    {
        /*[SerializeField]
        [Tooltip("Auto-scrolls the list up and down in a sin wave.")]
        private bool sinScroll = true;*/
        
        [SerializeField] AppManager appManager;
        [SerializeField] GameObject warehouse;

        private VirtualizedScrollRectList list;
        private float destScroll;
        private bool animate;
        private bool forDeposit = false;
        private List<GameObject> depositList = new();
        private readonly string depositText = "Navigare la gerarchia fino allo scaffale desiderato";

        private readonly string[] words = { "one", "two", "three", "zebra", "keyboard", "rabbit", "graphite", "ruby", };
        private List<string> buttonsNames = new();

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            if (buttonsNames.Count > 0)
            {
                list = GetComponent<VirtualizedScrollRectList>();
                list.OnVisible = (go, i) =>
                {
                    foreach (var text in go.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        if (text.gameObject.name == "Text")
                        {
                            text.text = $"{buttonsNames[i % buttonsNames.Count]}";
                        }
                    }

                    foreach (var item in go.GetComponentsInChildren<PressableButton>())
                    {
                        item.OnClicked.AddListener(() => ButtonListener(i));
                    }
                };

                list.OnInvisible = (go, i) =>
                {
                    foreach (var item in go.GetComponentsInChildren<PressableButton>())
                    {
                        item.OnClicked.RemoveAllListeners();
                    }
                };
            }
            else
            {
                list = GetComponent<VirtualizedScrollRectList>();
                list.OnVisible = (go, i) =>
                {
                    foreach (var text in go.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        if (text.gameObject.name == "Text")
                        {
                            text.text = $"{words[i % words.Length]}";
                        }
                    }

                    foreach (var item in go.GetComponentsInChildren<PressableButton>())
                    {
                        item.OnClicked.AddListener(() => ButtonListener(i));
                    }
                };

                list.OnInvisible = (go, i) =>
                {
                    foreach (var item in go.GetComponentsInChildren<PressableButton>())
                    {
                        item.OnClicked.RemoveAllListeners();
                    }
                };
            }
            
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            /*if (sinScroll)
            {
                list.Scroll = (Mathf.Sin(Time.time * 0.5f - (Mathf.PI / 2)) * 0.5f + 0.5f) * list.MaxScroll;
                destScroll = list.Scroll;
                animate = false;
            }*/

            if (animate)
            {
                float newScroll = Mathf.Lerp(list.Scroll, destScroll, 8 * Time.deltaTime);
                list.Scroll = newScroll;
                if (Mathf.Abs(list.Scroll - destScroll) < 0.02f)
                {
                    list.Scroll = destScroll;
                    animate = false;
                }
            }
        }

        /// <summary>
        /// Scrolls the VirtualizedScrollRect to the next page.
        /// </summary>
        public void Next()
        {
            //sinScroll = false;
            animate = true;
            destScroll = Mathf.Min(list.MaxScroll, Mathf.Floor(list.Scroll / list.RowsOrColumns) * list.RowsOrColumns + list.TotallyVisibleCount);
        }

        /// <summary>
        /// Scrolls the VirtualizedScrollRect to the previous page.
        /// </summary>
        public void Prev()
        {
            //sinScroll = false;
            animate = true;
            destScroll = Mathf.Max(0, Mathf.Floor(list.Scroll / list.RowsOrColumns) * list.RowsOrColumns - list.TotallyVisibleCount);
        }

        /// <summary>
        /// Testing function for adjusting the number of items during
        /// runtime.
        /// </summary>
        [ContextMenu("Set Item Count 50")]
        public void TestItemCount1() => list.SetItemCount(50);

        /// <summary>
        /// Testing function for adjusting the number of items during
        /// runtime.
        /// </summary>
        [ContextMenu("Set Item Count 200")]
        public void TestItemCount2() => list.SetItemCount(200);

        void ButtonListener(int i)
        {
            Debug.Log("Bottone selezionato: " + buttonsNames[i]);

            if (!forDeposit)
            {
                if (appManager.A_Menu.artifactsPanel.activeSelf)
                {
                    appManager.OnArtifactButtonClicked(i);
                }
                else if (appManager.shelvesListPanel.activeSelf)
                {
                    appManager.OnShelfButtonClicked(i);
                }
            }
            else
            {
                if (depositList[i].transform.childCount > 0)
                    ListForDeposit(depositList[i]);
                else
                {
                    Debug.Log("Reperto posizionato");
                    GameObject artifact = appManager.GetArtifactSelected();
                    artifact.GetComponent<Artifact>().SetShelfID(depositList[i].name);
                    PlayerPrefs.SetString(artifact.name, depositList[i].name);

                    forDeposit = false;
                    appManager.BackButtonArtifact();
                }
            }
            
        }

        public void SetWords(List<GameObject> items)
        {
            list = GetComponent<VirtualizedScrollRectList>();
            list.SetItemCount(0);
            buttonsNames.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                buttonsNames.Add(items[i].name);
            }
            
            list.SetItemCount(items.Count);
            this.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        }

        public void ListForDeposit(GameObject parent)
        {
            forDeposit = true;
            depositList.Clear();

            if (parent == warehouse)
                appManager.A_Menu.artifactText.GetComponent<TextMeshProUGUI>().text = depositText;

            for (int i = 0; i < parent.transform.childCount; i++)
                depositList.Add(parent.transform.GetChild(i).gameObject);

            depositList.Sort((x,y) => x.name.CompareTo(y.name));

            SetWords(depositList);
        }

        public void Back()
        {
            GameObject currentParent = depositList[0].transform.parent.gameObject;

            if (currentParent != null)
            {
                if(currentParent.name != warehouse.name)
                {
                    ListForDeposit(currentParent.transform.parent.gameObject);
                }
                else
                {
                    forDeposit = false;
                    appManager.BackButtonArtifact();
                }
            }
        }

        public bool GetForDeposit()
            { return forDeposit; }
    }
}
#pragma warning restore CS1591
