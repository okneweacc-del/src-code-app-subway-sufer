using System.Diagnostics;
using System.Text;
using Kiloo.Common.Backend.PurchaseValidation;
using UnityEngine;

namespace InAppPurchase.Internal;

public class AndroidPurchaseProvider : IPurchaseProvider
{
	private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "AndroidPurchaseBridge_callback";

	private const string FINISHED_TRANSACTION_ID_PREFIX = "SHOULD_CONSUME_";

	private const string FINISHED_PURCHASE_TOKEN_PREFIX = "SHOULD_CONSUME_TOKEN_";

	private const string SEPARATOR_CHAR = ";";

	private static readonly char[] SEPARATOR_FOR_STRING_SPLIT = new char[1] { ';' };

	private InitDelegate _initDelegate;

	private PurchaseDelegate _purchaseDelegate;

	private ItemRestoredDelegate _itemRestoredDelegate;

	private CheckedItemRestoredDelegate _checkedItemRestoredDelegate;

	private ItemRefundedDelegate _itemRefundedDelegate;

	private ProductInfoRequestDelegate _productRequestDelegate;

	private RestorePurchaseFinishedDelegate _restorePurchaseFinishedDelegate;

	private IInAppManager _inAppManager;

	private IPurchaseValidationServer _purchaseValidationServer;

	private string _iapAPIKey;

	private AndroidJavaObject _iabInstance;

	private GameObject _callbackGameObject;

	public bool IsValidationServerSpecified => _purchaseValidationServer != null;

	public void Init(IInAppManager inAppManager, IPurchaseValidationServer purchaseValidationServer)
	{
		_inAppManager = inAppManager;
		_initDelegate = inAppManager.InitFinishedCallback;
		_purchaseDelegate = inAppManager.PurchaseRequestCallback;
		_productRequestDelegate = inAppManager.ProductInfoRequestCallback;
		_itemRestoredDelegate = inAppManager.ItemRestoredCallback;
		_checkedItemRestoredDelegate = inAppManager.CheckedItemRestoredCallback;
		_itemRefundedDelegate = inAppManager.ItemRefundedCallback;
		_restorePurchaseFinishedDelegate = inAppManager.RestoreRequestFinishedCallback;
		_purchaseValidationServer = purchaseValidationServer;
		_iapAPIKey = inAppManager.getInAppAPIKey();
		if (_iabInstance == null)
		{
			_iabInstance = new AndroidJavaObject("com.kiloo.androidiablibrary.IABBridge");
		}
		_iabInstance.Set("debugLogsEnabled", val: false);
		_iabInstance.Set("shouldAutorestoreOnProductRequest", val: false);
		if (_callbackGameObject == null)
		{
			_callbackGameObject = new GameObject("AndroidPurchaseBridge_callback");
			Object.DontDestroyOnLoad(_callbackGameObject);
			AndroidPurchaseProviderCallbackListener androidPurchaseProviderCallbackListener = _callbackGameObject.AddComponent<AndroidPurchaseProviderCallbackListener>();
			androidPurchaseProviderCallbackListener.Setup(InitSuccessful, InitFailed, ProductRequestSuccessful, ProductRequestFailed, PurchasePurchased, PurchaseRestored, PurchaseRefunded, PurchaseFailed, RestoreFinished, ConsumeFinished);
		}
		_iabInstance.Call("init", _iapAPIKey);
	}

	public bool CanMakePurchases()
	{
		return _iabInstance.Call<bool>("isIabPossible", new object[0]);
	}

	public void RequestProductInfo(string[] productIds)
	{
		string text = string.Join(";", productIds);
		string text2 = string.Empty;
		foreach (string text3 in productIds)
		{
			text2 = text2 + (_inAppManager.IsProductConsumable(text3) == IsIAPConsumableValue.Consumable) + ";";
			if (!text3.Equals(text3.ToLower()))
			{
				LogError("The product id: " + text3 + " contains uppercase letters. This is an error, as it will not be accepted by Google Play!");
			}
		}
		_iabInstance.Call("requestProductInfo", text, text2);
	}

	public bool StartPurchase(string productId)
	{
		return _iabInstance.Call<bool>("startPurchase", new object[1] { productId });
	}

	public bool StartPurchase(string productId, string cookie)
	{
		return _iabInstance.Call<bool>("startPurchase", new object[2] { productId, cookie });
	}

	public void FinishPurchase(string productId, string transactionId, string purchaseToken)
	{
		if (_inAppManager.IsProductConsumable(productId) == IsIAPConsumableValue.Consumable)
		{
			if (!string.IsNullOrEmpty(transactionId))
			{
				PlayerPrefs.SetString("SHOULD_CONSUME_" + transactionId, productId);
			}
			if (!string.IsNullOrEmpty(purchaseToken))
			{
				PlayerPrefs.SetString("SHOULD_CONSUME_TOKEN_" + purchaseToken, productId);
			}
			PlayerPrefs.Save();
		}
		_iabInstance.Call("finishPurchase", productId, transactionId, purchaseToken);
	}

	private string ProductIDsAsString(string[] productIds)
	{
		return string.Join(";", productIds);
	}

	private string ProductIsConsumableAsString(string[] productIds)
	{
		string text = string.Empty;
		foreach (string productID in productIds)
		{
			text = text + (_inAppManager.IsProductConsumable(productID) == IsIAPConsumableValue.Consumable) + ";";
		}
		return text;
	}

	public bool RestoreNonConsumablePurchases(string[] productIds)
	{
		return _iabInstance.Call<bool>("restoreNonConsumablePurchases", new object[2]
		{
			ProductIDsAsString(productIds),
			ProductIsConsumableAsString(productIds)
		});
	}

	private void InitSuccessful()
	{
		_initDelegate?.Invoke(success: true);
	}

	private void InitFailed()
	{
		_initDelegate?.Invoke(success: false);
	}

	private void ProductRequestSuccessful(string message)
	{
		if (TryParseProductInfo(message, out var productInfo))
		{
			StringBuilder stringBuilder = new StringBuilder("AndroidPurchaseProvider ProductInfo:\n");
			ProductRequestResultInfo[] array = productInfo;
			for (int i = 0; i < array.Length; i++)
			{
				ProductRequestResultInfo productRequestResultInfo = array[i];
				stringBuilder.AppendLine("    productId: " + productRequestResultInfo.productId + "  localized price: " + productRequestResultInfo.localizedPrice);
			}
			_productRequestDelegate?.Invoke(success: true, productInfo);
		}
		else
		{
			LogError("AndroidPurchaseProvider ERROR: Malformed message from native code: " + message);
		}
	}

	private void ProductRequestFailed()
	{
		_productRequestDelegate?.Invoke(success: false, new ProductRequestResultInfo[0]);
	}

	private void PurchasePurchased(string message)
	{
		HandlePurchaseEnded(message, InternalPurchaseResult.Completed);
	}

	private void PurchaseRestored(string message)
	{
		HandlePurchaseEnded(message, InternalPurchaseResult.Restored);
	}

	private void PurchaseRefunded(string message)
	{
		HandlePurchaseEnded(message, InternalPurchaseResult.Refunded);
	}

	private void PurchaseFailed(string message)
	{
		string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
		InternalPurchaseResult internalPurchaseResult = InternalPurchaseResult.Failed;
		if (array.Length == 6)
		{
			internalPurchaseResult = GetProperPurchaseResult(array[5]);
			message = array[0] + ";" + array[1] + ";" + array[2] + ";" + array[3] + ";" + array[4];
		}
		HandlePurchaseEnded(message, internalPurchaseResult);
	}

	private void RestoreFinished(string message)
	{
		int.TryParse(message, out var result);
		RestoreFinishedResult result2 = result switch
		{
			0 => RestoreFinishedResult.Failed, 
			1 => RestoreFinishedResult.NothingToRestore, 
			2 => RestoreFinishedResult.Completed, 
			_ => RestoreFinishedResult.Failed, 
		};
		_restorePurchaseFinishedDelegate?.Invoke(result2);
	}

	private void ConsumeFinished(string message)
	{
		string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
		string text = string.Empty;
		string text2 = string.Empty;
		if (array.Length != 2)
		{
			LogError("Unexpected length of message in ConsumeFinished, expected 2, received " + array.Length);
		}
		else
		{
			text = array[0];
			text2 = array[1];
		}
		string value = PlayerPrefs.GetString("SHOULD_CONSUME_" + text);
		string value2 = PlayerPrefs.GetString("SHOULD_CONSUME_TOKEN_" + text2);
		bool flag = false;
		if (!string.IsNullOrEmpty(value))
		{
			PlayerPrefs.DeleteKey("SHOULD_CONSUME_" + text);
			flag = true;
		}
		if (!string.IsNullOrEmpty(value2))
		{
			PlayerPrefs.DeleteKey("SHOULD_CONSUME_TOKEN_" + text2);
			flag = true;
		}
		if (flag)
		{
			PlayerPrefs.Save();
		}
	}

	private void HandlePurchaseEnded(string message, InternalPurchaseResult internalPurchaseResult)
	{
		if (TryParsePurchaseEndedMessage(message, out var productId, out var transactionId, out var _, out var cookie, out var receipt))
		{
			switch (internalPurchaseResult)
			{
			case InternalPurchaseResult.Restored:
				HandleRestore(productId, transactionId, cookie, receipt);
				return;
			case InternalPurchaseResult.Refunded:
				HandleRefund(productId, transactionId, cookie);
				return;
			case InternalPurchaseResult.Canceled:
				HandleCancel(productId, transactionId, cookie, receipt);
				return;
			}
			if (HasTransactionBeenMadeInGame(productId, transactionId))
			{
				HandleDefaultCases(internalPurchaseResult, productId, transactionId, cookie, receipt);
			}
			else
			{
				FinishPurchase(productId, transactionId, receipt);
			}
		}
		else
		{
			LogError("AndroidPurchaseProvider ERROR: Malformed message from native code: " + message);
		}
	}

	private bool HasTransactionBeenMadeInGame(string productId, string transactionId)
	{
		string value = PlayerPrefs.GetString("SHOULD_CONSUME_" + transactionId);
		return _inAppManager.IsProductConsumable(productId) == IsIAPConsumableValue.NotConsumable || string.IsNullOrEmpty(value);
	}

	private void HandleRestore(string productId, string transactionId, string cookie, string receipt)
	{
		if (_inAppManager.IsProductConsumable(productId) == IsIAPConsumableValue.Consumable)
		{
			FinishPurchase(productId, transactionId, receipt);
		}
		InvokeItemRestore(productId, transactionId, cookie);
		InvokeCheckedItemRestore(productId, transactionId, cookie, receipt);
	}

	private void InvokeItemRestore(string productId, string transactionId, string cookie)
	{
		_itemRestoredDelegate?.Invoke(productId, transactionId, cookie);
	}

	private void InvokeCheckedItemRestore(string productId, string transactionId, string cookie, string receipt)
	{
		CheckedItemRestoredDelegate handler = _checkedItemRestoredDelegate;
		if (handler != null && _purchaseValidationServer != null)
		{
			_purchaseValidationServer.ValidateTransaction(productId, receipt, (IPurchaseValidationResult validationResult) =>
			{
				handler(productId, transactionId, cookie, validationResult);
			});
		}
	}

	private void HandleRefund(string productId, string transactionId, string cookie)
	{
		_itemRefundedDelegate?.Invoke(productId, transactionId, cookie, null);
	}

	private void HandleCancel(string productId, string transactionId, string cookie, string receipt)
	{
		_purchaseDelegate?.Invoke(productId, transactionId, receipt, cookie, PurchaseResult.Canceled, null);
	}

	private void HandleDefaultCases(InternalPurchaseResult internalPurchaseResult, string productId, string transactionId, string cookie, string receipt)
	{
		if (internalPurchaseResult == InternalPurchaseResult.Completed)
		{
			HandlePurchase(productId, transactionId, cookie, receipt, internalPurchaseResult);
		}
		else
		{
			_purchaseDelegate?.Invoke(productId, transactionId, receipt, cookie, internalPurchaseResult.ToPurchaseResult(), null);
		}
	}

	private void HandlePurchase(string productId, string transactionId, string cookie, string receipt, InternalPurchaseResult internalPurchaseResult)
	{
		string value = PlayerPrefs.GetString("SHOULD_CONSUME_" + transactionId);
		if (_inAppManager.IsProductConsumable(productId) == IsIAPConsumableValue.NotConsumable || string.IsNullOrEmpty(value))
		{
			if (_purchaseValidationServer != null)
			{
				_purchaseValidationServer.ValidateTransaction(productId, receipt, (IPurchaseValidationResult validationResult) =>
				{
					_purchaseDelegate?.Invoke(productId, transactionId, receipt, cookie, internalPurchaseResult.ToPurchaseResult(), validationResult);
				});
			}
			else
			{
				_purchaseDelegate?.Invoke(productId, transactionId, receipt, cookie, internalPurchaseResult.ToPurchaseResult(), null);
			}
		}
		else
		{
			FinishPurchase(productId, transactionId, receipt);
		}
	}

	private InternalPurchaseResult GetProperPurchaseResult(string errorCode)
	{
		if (int.TryParse(errorCode, out var result))
		{
			return result switch
			{
				0 => InternalPurchaseResult.Completed, 
				1 => InternalPurchaseResult.Failed, 
				2 => InternalPurchaseResult.Canceled, 
				3 => InternalPurchaseResult.AlreadyOwned, 
				4 => InternalPurchaseResult.ItemUnavailable, 
				_ => InternalPurchaseResult.Failed, 
			};
		}
		return InternalPurchaseResult.Failed;
	}

	private bool TryParseProductInfo(string message, out ProductRequestResultInfo[] productInfo)
	{
		string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
		if (array.Length >= 1 && int.TryParse(array[0], out var result))
		{
			int num = 1 + result * 4;
			if (array.Length == num)
			{
				ProductRequestResultInfo[] array2 = new ProductRequestResultInfo[result];
				for (int i = 0; i < result; i++)
				{
					int num2 = 1 + i * 4;
					string productId = array[num2];
					string localizedPrice = array[num2 + 1];
					string title = array[num2 + 2];
					string description = array[num2 + 3];
					array2[i].Init(productId, localizedPrice, title, description);
				}
				productInfo = array2;
				return true;
			}
		}
		productInfo = new ProductRequestResultInfo[0];
		return false;
	}

	private bool TryParsePurchaseEndedMessage(string message, out string productId, out string transactionId, out string purchaseSignature, out string cookie, out string receipt)
	{
		string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
		if (array.Length == 5)
		{
			productId = array[0];
			transactionId = array[1];
			purchaseSignature = array[2];
			cookie = array[3];
			receipt = array[4];
			return true;
		}
		productId = string.Empty;
		transactionId = string.Empty;
		purchaseSignature = string.Empty;
		cookie = string.Empty;
		receipt = string.Empty;
		return false;
	}

	private static void LogError(string msg)
	{
		SDebug.LogError("[AndroidPurchaseProvider] " + msg);
	}

	[Conditional("IAP_DEBUG_LOGS")]
	private static void Log(string msg)
	{
	}
}
