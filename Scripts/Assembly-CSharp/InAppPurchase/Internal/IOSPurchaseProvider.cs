using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using Kiloo.Common.Backend.PurchaseValidation;
using UnityEngine;

namespace InAppPurchase.Internal;

public class IOSPurchaseProvider : IPurchaseProvider
{
	private const string BRIDGE_DELEGATE_GAMEOBJECT_NAME = "IOSPurchaseBridge_callback";

	private const char SEPARATOR_CHAR = ';';

	private static readonly char[] SEPARATOR_FOR_STRING_SPLIT = new char[1] { ';' };

	private IInAppManager _inAppManager;

	private PurchaseDelegate _purchaseDelegate;

	private ItemRestoredDelegate _itemRestoredDelegate;

	private CheckedItemRestoredDelegate _checkedItemRestoredDelegate;

	private ProductInfoRequestDelegate _productRequestDelegate;

	private RestorePurchaseFinishedDelegate _restorePurchaseFinishedDelegate;

	private IPurchaseValidationServer _purchaseValidationServer;

	private GameObject _callbackGameObject;

	public bool IsValidationServerSpecified => _purchaseValidationServer != null;

	public void Init(IInAppManager inAppManager, IPurchaseValidationServer purchaseValidationServer)
	{
		_inAppManager = inAppManager;
		_purchaseDelegate = inAppManager.PurchaseRequestCallback;
		_productRequestDelegate = inAppManager.ProductInfoRequestCallback;
		_itemRestoredDelegate = inAppManager.ItemRestoredCallback;
		_checkedItemRestoredDelegate = inAppManager.CheckedItemRestoredCallback;
		_restorePurchaseFinishedDelegate = inAppManager.RestoreRequestFinishedCallback;
		_purchaseValidationServer = purchaseValidationServer;
		IOSPurchaseBridge_EnableDebugLogs("false");
		if (_callbackGameObject == null)
		{
			_callbackGameObject = new GameObject("IOSPurchaseBridge_callback");
			UnityEngine.Object.DontDestroyOnLoad(_callbackGameObject);
			IOSPurchaseProviderCallbackListener iOSPurchaseProviderCallbackListener = _callbackGameObject.AddComponent<IOSPurchaseProviderCallbackListener>();
			iOSPurchaseProviderCallbackListener.Setup(ProductRequestSuccessful, ProductRequestFailed, PurchasePurchased, PurchaseRestored, PurchaseFailed, RestoreFinished);
		}
		IOSPurchaseBridge_Init();
		_inAppManager.InitFinishedCallback(success: true);
	}

	public bool CanMakePurchases()
	{
		return IOSPurchaseBridge_CanMakePayments();
	}

	public void RequestProductInfo(string[] productIds)
	{
		IOSPurchaseBridge_RunProductsRequest(ArrayToString(productIds));
	}

	public bool StartPurchase(string productId)
	{
		return StartPurchase(productId, null);
	}

	public bool StartPurchase(string productId, string cookie)
	{
		if (string.IsNullOrEmpty(cookie))
		{
			LogWarning("Cookies for the purchase is not supported for iOS.");
		}
		return IOSPurchaseBridge_StartPurchase(productId);
	}

	public void FinishPurchase(string productId, string transactionId, string purchaseToken)
	{
		IOSPurchaseBridge_FinishTransaction(productId, transactionId);
	}

	public bool RestoreNonConsumablePurchases(string[] productIds)
	{
		return IOSPurchaseBridge_RestoreNonConsumablePurchases();
	}

	private string ArrayToString(string[] array)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i]);
			if (i < array.Length - 1)
			{
				stringBuilder.Append(';');
			}
		}
		return stringBuilder.ToString();
	}

	private void HandlePurchaseEnded(string message, InternalPurchaseResult internalPurchaseResult)
	{
		if (TryParsePurchaseEndedMessage(message, out var productId, out var transactionId, out var transactionRecieptBase))
		{
			switch (internalPurchaseResult)
			{
			case InternalPurchaseResult.Completed:
				HandlePurchase(productId, transactionId, transactionRecieptBase, internalPurchaseResult);
				break;
			case InternalPurchaseResult.Restored:
				HandleRestore(productId, transactionId, transactionRecieptBase);
				break;
			default:
				_purchaseDelegate?.Invoke(productId, transactionId, transactionRecieptBase, null, internalPurchaseResult.ToPurchaseResult(), null);
				break;
			}
		}
		else
		{
			LogError("IOSPurchaseProvider ERROR: Malformed message from native code: " + message);
		}
	}

	private void HandlePurchase(string productId, string transactionId, string transactionRecieptBase64, InternalPurchaseResult internalPurchaseResult)
	{
		ValidateTransaction(productId, transactionRecieptBase64, (IPurchaseValidationResult validationResult) =>
		{
			_purchaseDelegate?.Invoke(productId, transactionId, transactionRecieptBase64, null, internalPurchaseResult.ToPurchaseResult(), validationResult);
		});
	}

	private void HandleRestore(string productId, string transactionId, string transactionRecieptBase64)
	{
		InvokeItemRestore(productId, transactionId);
		InvokeCheckedItemRestore(productId, transactionId, transactionRecieptBase64);
	}

	private void InvokeItemRestore(string productId, string transactionId)
	{
		_itemRestoredDelegate?.Invoke(productId, transactionId, null);
	}

	private void InvokeCheckedItemRestore(string productId, string transactionId, string transactionRecieptBase64)
	{
		CheckedItemRestoredDelegate handler = _checkedItemRestoredDelegate;
		if (handler != null && _purchaseValidationServer != null)
		{
			ValidateTransaction(productId, transactionRecieptBase64, (IPurchaseValidationResult validationResult) =>
			{
				handler(productId, transactionId, null, validationResult);
			});
		}
	}

	private void ValidateTransaction(string productId, string transactionReciptBase64, [NotNull] Action<IPurchaseValidationResult> completeHandler)
	{
		if (_purchaseValidationServer != null)
		{
			_purchaseValidationServer.ValidateTransaction(productId, transactionReciptBase64, completeHandler);
		}
	}

	private bool TryParsePurchaseEndedMessage(string message, out string productId, out string transactionId, out string transactionRecieptBase64)
	{
		string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
		if (array.Length == 3)
		{
			productId = array[0];
			transactionId = array[1];
			transactionRecieptBase64 = array[2];
			return true;
		}
		productId = string.Empty;
		transactionId = string.Empty;
		transactionRecieptBase64 = string.Empty;
		return false;
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

	private void ProductRequestSuccessful(string message)
	{
		if (TryParseProductInfo(message, out var productInfo))
		{
			StringBuilder stringBuilder = new StringBuilder("IOSPurchaseProvider ProductInfo:\n");
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
			LogError("IOSPurchaseProvider ERROR: Malformed message from native code: " + message);
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

	private void PurchaseFailed(string message)
	{
		string[] array = message.Split(SEPARATOR_FOR_STRING_SPLIT);
		InternalPurchaseResult internalPurchaseResult = InternalPurchaseResult.Failed;
		if (array.Length == 4)
		{
			internalPurchaseResult = GetProperPurchaseResult(array[3]);
			message = array[0] + ';' + array[1] + ';' + array[2];
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
			3 => RestoreFinishedResult.Canceled, 
			_ => RestoreFinishedResult.Failed, 
		};
		_restorePurchaseFinishedDelegate?.Invoke(result2);
	}

	private static void LogError(string msg)
	{
		SDebug.LogError("[IOSPurchaseProvider] " + msg);
	}

	private static void LogWarning(string msg)
	{
	}

	[Conditional("IAP_DEBUG_LOGS")]
	private static void Log(string msg)
	{
	}

	private static void IOSPurchaseBridge_Init()
	{
	}

	private static bool IOSPurchaseBridge_CanMakePayments()
	{
		return false;
	}

	private static void IOSPurchaseBridge_RunProductsRequest(string productIdsList)
	{
	}

	private static bool IOSPurchaseBridge_StartPurchase(string productId)
	{
		return false;
	}

	private static void IOSPurchaseBridge_FinishTransaction(string productId, string transactionId)
	{
	}

	private static bool IOSPurchaseBridge_RestoreNonConsumablePurchases()
	{
		return false;
	}

	private static bool IOSPurchaseBridge_EnableDebugLogs(string enabledBoolString)
	{
		return false;
	}
}
