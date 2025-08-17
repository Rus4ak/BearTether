using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.SceneManagement;

public class Hardcore : MonoBehaviour, IDetailedStoreListener, IStoreListener
{
    [SerializeField] private GameObject _buyMenu;
    [SerializeField] private TextMeshProUGUI _priceText;

    [Header("Multiplayer")]
    [SerializeField] private GameObject _normalLevelsMenu;
    [SerializeField] private GameObject _hardcoreLevelsMenu;

    private IStoreController _storeController;

    private string _product_id = "hardcore";

    //public static bool isBoughtHardcore;

    private void Start()
    {
        if (_storeController == null)
            InitializePurchasing();
    }

    private void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(_product_id, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void SwitchHardcore(string SceneName)
    {
        AnalyticsManager.Instance.PressHardcore();

        if (CheckHardcore())
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            _buyMenu.SetActive(true);
        }
    }

    public void SwitchHardcoreMultiplayer()
    {
        AnalyticsManager.Instance.PressHardcore();
        if (CheckHardcore())
        {
            if (_normalLevelsMenu.activeInHierarchy)
            {
                _hardcoreLevelsMenu.SetActive(true);
                _normalLevelsMenu.SetActive(false);
            }
            else
            {
                _normalLevelsMenu.SetActive(true);
                _hardcoreLevelsMenu.SetActive(false);
            }
        }
        else
        {
            _buyMenu.SetActive(true);
        }

    }

    public void BuyHardcore()
    {
        AnalyticsManager.Instance.PressBuyHardcore();
        _storeController.InitiatePurchase(_product_id);
    }

    private bool CheckHardcore()
    {
        if (_storeController!=null)
        {
            var product = _storeController.products.WithID(_product_id);
            if (product != null)
            {
                if (product.hasReceipt)
                    return true;
                else
                    return false; 
            }
        }

        return false;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError("Purchase failed: " + failureDescription);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP init failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("IAP init failed: " + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (purchaseEvent.purchasedProduct.definition.id == _product_id)
        {
            //isBoughtHardcore = true;
            //Progress.Instance.progressData.isBoughtHardcore = true;
            //Progress.Instance.Save();
            _buyMenu.SetActive(false);
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Purchase failed: " + failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;

        Product product = _storeController.products.WithID(_product_id);

        if (product != null)
        {
            _priceText.text = product.metadata.localizedPriceString;
        }
    }
}
