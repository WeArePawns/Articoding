/****************************************************************************

Copyright 2016 sophieml1989@gmail.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

****************************************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UBlockly.UGUI
{
    public class ClassicToolbox : BaseToolbox
    {
        [SerializeField] protected GameObject m_MenuItemPrefab;
        [SerializeField] protected RectTransform m_MenuListContent;
        [SerializeField] protected GameObject m_BlockScrollList;
        [SerializeField] protected GameObject m_BlockContentPrefab;
        [SerializeField] protected GameObject m_BinArea;

        protected override void Build()
        {
            BuildMenu();
        }

        /// <summary>
        /// Build the left menu list, child class should implement this for custom build
        /// </summary>
        protected virtual void BuildMenu()
        {
            foreach (var category in mConfig.BlockCategoryList)
            {
                GameObject menuItem = GameObject.Instantiate(m_MenuItemPrefab, m_MenuListContent, false);
                menuItem.name = category.CategoryName;
                menuItem.GetComponentInChildren<Text>().text = I18n.Get(category.CategoryName);
                Image[] images = menuItem.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = category.Color;
                }
                menuItem.SetActive(false);

                Toggle toggle = menuItem.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener((selected) =>
                {
                    if (selected)
                        ShowBlockCategory(menuItem.name);
                    else
                        HideBlockCategory();
                });
                mMenuList[category.CategoryName] = toggle;
            }
        }

        public void ShowBlockCategory(string categoryName)
        {
            if (string.Equals(categoryName, mActiveCategory))
                return;

            if (!m_BlockScrollList.activeInHierarchy)
                m_BlockScrollList.SetActive(true);

            if (!string.IsNullOrEmpty(mActiveCategory))
                mRootList[mActiveCategory].SetActive(false);

            mActiveCategory = categoryName;

            GameObject contentObj;
            RectTransform contentTrans;
            if (mRootList.TryGetValue(categoryName, out contentObj))
            {
                contentObj.SetActive(true);
                contentTrans = contentObj.transform as RectTransform;
            }
            else
            {
                contentObj = GameObject.Instantiate(m_BlockContentPrefab, m_BlockContentPrefab.transform.parent);
                contentObj.name = "Content_" + categoryName;
                contentObj.SetActive(true);
                mRootList[categoryName] = contentObj;

                contentTrans = contentObj.GetComponent<RectTransform>();

                //build new blocks
                if (categoryName.Equals(Define.VARIABLE_CATEGORY_NAME))
                    BuildVariableBlocks();
                else if (categoryName.Equals(Define.PROCEDURE_CATEGORY_NAME))
                    BuildProcedureBlocks();
                else
                    BuildBlockViewsForActiveCategory();
            }

            //resize the background
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTrans);
            m_BlockScrollList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LayoutUtility.GetPreferredWidth(contentTrans));

            m_BlockScrollList.GetComponent<ScrollRect>().content = contentTrans;
        }

        public void HideBlockCategory()
        {
            if (string.IsNullOrEmpty(mActiveCategory))
                return;

            mRootList[mActiveCategory].SetActive(false);
            mMenuList[mActiveCategory].isOn = false;
            m_BlockScrollList.SetActive(false);
            mActiveCategory = null;
        }

        /// <summary>
        /// Build block views for the active category, child class should implement this for custom build
        /// </summary>
        protected virtual void BuildBlockViewsForActiveCategory()
        {
            Transform contentTrans = mRootList[mActiveCategory].transform;
            var blockTypes = mConfig.GetBlockCategory(mActiveCategory).BlockList;
            foreach (string blockType in blockTypes)
            {
                BlockView block = NewBlockView(blockType, contentTrans);

                //Deactivate the block if it's not in the active list
                bool active = false;
                if (activeCategories != null && activeCategories.ContainsKey(mActiveCategory.ToLower()))
                {
                    CategoryBlocks info = activeCategories[mActiveCategory.ToLower()];
                    active = info.activate == (info.activeBlocks.ContainsKey(blockType));
                    int prevValue = (Block.blocksAvailable.ContainsKey(blockType)) ? Block.blocksAvailable[blockType] : 0;
                    int value = prevValue + (info.activeBlocks.ContainsKey(blockType) ? info.activeBlocks[blockType] : Int16.MaxValue);
                    Block.blocksAvailable[blockType] = value;
                }
                block.gameObject.SetActive(allActive || active);
                block.UpdateCount();
            }
        }

        protected override void OnPickBlockView()
        {
            HideBlockCategory();
        }

        public override bool CheckBin(BlockView blockView)
        {
            if (blockView.InToolbox) return false;

            RectTransform toggleTrans = m_BinArea.transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(toggleTrans, UnityEngine.Input.mousePosition, BlocklyUI.UICanvas.worldCamera))
            {
                m_BinArea.gameObject.SetActive(true);
                return true;
            }
            m_BinArea.gameObject.SetActive(false);
            return false;
        }

        public override void FinishCheckBin(BlockView blockView)
        {
            if (CheckBin(blockView))
            {
                string type = blockView.BlockType;
                string category = GetCategoryNameOfBlockView(blockView);
                bool reactivate = Block.blocksAvailable.ContainsKey(type) && Block.blocksAvailable[type] == 0;
                blockView.Dispose();

                //Update the blockCounter
                foreach (BlockView bw in mRootList[category].transform.GetComponentsInChildren<BlockView>())
                    if (bw.BlockType == type)
                    {
                        //If the block was disabled we reactivate it
                        if (reactivate)
                        {
                            bw.enabled = true;
                            bw.ChangeBgColor(GetColorOfBlockView(bw));
                        }
                        bw.UpdateCount();
                        break;
                    }
            }
            m_BinArea.gameObject.SetActive(false);
        }
    }
}