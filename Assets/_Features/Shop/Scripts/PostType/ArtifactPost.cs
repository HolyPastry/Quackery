
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Artifacts;
using Quackery.Effects;
using Quackery.GameMenu;
using Quackery.TetrisBill;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Quackery.Shops
{
    public class ArtifactPost : ShopPost, ITooltipTarget, IPointerUpHandler
    {
        [SerializeField] TextMeshProUGUI _titleGUI;
        [SerializeField] TextMeshProUGUI _descriptionGUI;
        [SerializeField] TextMeshProUGUI _priceGUI;
        [SerializeField] Button _buyButton;
        [SerializeField] Image _picture;
        [SerializeField] TextMeshProUGUI _billTitleGUI;
        [SerializeField] GameObject _billPanel;
        [SerializeField] Transform _billBlockParent;

        [SerializeField] GameObject _artifactPanel;
        [SerializeField] Image _artifactIcon;
        [SerializeField] TextMeshProUGUI _artifactEffectGUI;
        private ArtifactReward _artifactReward;

        public List<Explanation> Explanations => _artifactReward.ArtifactData.Explanations;

        public RectTransform RectTransform => transform as RectTransform;

        void OnEnable()
        {
            _buyButton.onClick.AddListener(() => StartCoroutine(ArtifactRewardRoutine()));
            PurseEvents.OnPurseUpdated += UpdateBuyButton;
        }
        void OnDisable()
        {
            _buyButton.onClick.RemoveAllListeners();
            PurseEvents.OnPurseUpdated -= UpdateBuyButton;
        }

        private void UpdateBuyButton(float obj)
        {
            if (_artifactReward != null)
                _buyButton.interactable = PurseServices.CanAfford(_artifactReward.Price);
        }

        public override void SetupPost(ShopReward shopReward)
        {
            base.SetupPost(shopReward);
            _artifactReward = shopReward as ArtifactReward;

            _titleGUI.text = _artifactReward.Title;
            _descriptionGUI.text = _artifactReward.Description;
            _priceGUI.text = $"<sprite name=Money>{_artifactReward.Price}";
            _buyButton.interactable = PurseServices.CanAfford(_artifactReward.Price);
            _picture.sprite = _artifactReward.ArtifactData.ShopBanner;
            if (_artifactReward.ArtifactData.Bill == null)
            {
                _billPanel.SetActive(false);
            }
            else
            {
                _billPanel.SetActive(true);
                _billTitleGUI.text = _artifactReward.ArtifactData.Bill.MasterText;
                SpawnBillBlock(_artifactReward.ArtifactData.Bill.BlockPrefab);
            }
            _artifactIcon.sprite = _artifactReward.ArtifactData.Icon;
            _artifactEffectGUI.text = Sprites.Replace(_artifactReward.ArtifactData.Description);
        }

        private string FormatEffectText(List<Effect> effects)
        {
            string formattedText = "";
            foreach (var effect in effects)
            {
                if (!string.IsNullOrEmpty(formattedText))
                {
                    formattedText += "\n";
                }
                //formattedText += $"{effect.Description}";
            }
            return Sprites.Replace(formattedText);
        }

        private void SpawnBillBlock(TetrisBlock blockPrefab)
        {
            var prefab = Instantiate(blockPrefab, _billBlockParent);
            prefab.SetLogo(_artifactReward.ArtifactData.Bill.Icon);
        }

        private IEnumerator ArtifactRewardRoutine()
        {
            ArtifactServices.Add(_artifactReward.ArtifactData);


            _buyButton.gameObject.SetActive(false);
            yield return ShopApp.SpendMoneyRequest(_artifactReward.Price);

            yield return GameMenuController.AddToArtifactRequest(_artifactIcon.transform as RectTransform);

            yield return Tempo.WaitForABeat;
            if (_artifactReward.ArtifactData.Bill != null)
            {
                BillServices.AddNewBill(_artifactReward.ArtifactData.Bill, true);
                yield return GameMenuController.AddToBillsRequest(_billBlockParent as RectTransform);

                yield return Tempo.WaitForABeat;
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Tooltips.ShowTooltipRequest(this);
        }
    }
}
