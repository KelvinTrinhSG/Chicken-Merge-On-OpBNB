using Thirdweb;
using UDEV.DMTool;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.ChickenMerge
{
    public class ClaimLevelBonusDialog : Dialog
    {
        [SerializeField] private Text m_bonusAmountTxt;

        public override void Show()
        {
            base.Show();
            title.SetText($"LEVEL {UserDataHandler.Ins.curLevelId} CLEARED");
            if(m_bonusAmountTxt)
            {
                m_bonusAmountTxt.text = Helper.BigCurrencyFormat(GameController.Ins.LevelCoinBonus);
            }

            AdsController.Ins.OnUserReward.AddListener(ClaimDoubleEvent);
        }

        public void ClaimDouble()
        {
            AdsController.Ins.ShowRewardedVideo(); 
        }

        private void ClaimDoubleEvent()
        {
            ClaimHandle(2);
        }

        public void ClaimBtnEvent()
        {
            ClaimHandle();
        }

        private void ClaimHandle(int multier = 1)
        {
            AutoSpawnGun AutoSpawnGunReference = GameObject.Find("AutoSpawnGun").GetComponent<AutoSpawnGun>();
            AutoSpawnGunReference.TurnOffAutoSpawn();
            GameController.Ins.ClaimLevelBonus(multier);

            RewardDialog rewardDialog = (RewardDialog)DialogDB.Ins.GetDialog(DialogType.Reward);
            if (rewardDialog)
            {
                rewardDialog.AddCoinRewardItem(GameController.Ins.LevelCoinBonus *  multier);
                DialogDB.Ins.Show(rewardDialog);

                rewardDialog.onDialogCompleteClosed = () =>
                {
                    DialogDB.Ins.Show(DialogType.LevelCompleted);
                };

                GameController.Ins.CancelInvoke();
            }
        }

        private void OnDisable()
        {
            AdsController.Ins.OnUserReward.RemoveListener(ClaimDoubleEvent);
        }

        //Blockchain
        private ThirdwebSDK sdk;
        private string chestOpenContractAddress = "0x890522076D01d53Bd7Bf5485B97362a5BC57A9F4";

        public Button claimRewardTokenbtn;
        public Text claimRewardTokenbtnTxt;

        protected override void Start()
        {
            sdk = ThirdwebManager.Instance.SDK;
        }

        private void HideAllClaimBtn()
        {
            claimRewardTokenbtn.interactable = false;
        }

        private void ShowAllClaimBtn()
        {
            claimRewardTokenbtn.interactable = true;
        }

        private void ResetTextToNormal()
        {
            claimRewardTokenbtnTxt.text = "Claim Free";
        }

        public async void ClaimChestToken()
        {
            HideAllClaimBtn();
            claimRewardTokenbtnTxt.text = "Claiming...";
            Contract contract = sdk.GetContract(chestOpenContractAddress);
            var data = await contract.ERC20.Claim("1");
            Debug.Log("ChestOpen were claimed!");
            ClaimDoubleEvent();
            ShowAllClaimBtn();
            ResetTextToNormal();
        }

    }


}
