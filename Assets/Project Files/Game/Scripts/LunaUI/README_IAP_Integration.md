# IAP Store Integration - Luna UI & Legacy Store

Hệ thống tích hợp cho phép chuyển đổi giữa Legacy Store (UIStore) và Luna UI Store (IAPStoreView) một cách linh hoạt.

## 🏗️ Architecture Overview

### Các Components Chính

1. **IAPStoreView** - Luna UI Store implementation
2. **UIStoreManager** - Manager để chuyển đổi giữa các store
3. **UIStoreSwitcher** - Developer UI để test
4. **IAPStoreIntegrationDemo** - Demo script cho integration

### Luồng Hoạt Động

```
UIStoreManager 
    ├── Legacy Store (UIStore - uGUI)
    └── Luna UI Store (IAPStoreView - UI Toolkit)
         └── IAPManager Integration
             ├── ProductData binding
             ├── Price updates
             ├── Purchase handling
             └── Coin Packs System
                 ├── IAP Packs (Small/Medium/Large)
                 ├── Free Pack (24h cooldown)
                 └── AD Pack (Watch rewarded video)
```

## 🚀 Quick Start

### 1. Cơ Bản - Sử dụng UIStoreManager

```csharp
// Chuyển sang Luna UI Store
UIStoreManager.SetStoreType(UIStoreManager.StoreType.LunaUIStore);

// Hiển thị store hiện tại
UIStoreManager.ShowStore();

// Ẩn store
UIStoreManager.HideStore();

// Kiểm tra trạng thái
bool isOpen = UIStoreManager.IsStoreDisplayed();
```

### 2. Direct Control - Sử dụng trực tiếp

```csharp
// Luna UI Store
IAPStoreView.ShowLunaStore();
IAPStoreView.HideLunaStore();

// Legacy Store
UIController.ShowPage<UIStore>();
UIController.HidePage<UIStore>();
```

## 🔧 Setup & Configuration

### 1. Scene Setup

1. Thêm `IAPStoreView` GameObject vào scene
2. Gán `IAPStoreView.uxml` file vào UIDocument component
3. Thêm `UIStoreSwitcher` cho developer testing
4. Thêm `IAPStoreIntegrationDemo` để demo

### 2. IAPStoreView Configuration

```csharp
// IAPStoreView tự động:
// - Kết nối với IAPManager
// - Cập nhật giá real-time
// - Xử lý purchase events
// - Quản lý currency display
// - Handle coin packs system
```

### 3. Product Mapping

Luna UI buttons được map với ProductKeyType:
- `buy-no-ads` → `ProductKeyType.NoAds`
- `buy-starter-pack` → `ProductKeyType.StarterPack`
- `buy-power-pack` → `ProductKeyType.PUPack`

**Coin Packs Mapping:**
- `buy-coins-small` → `ProductKeyType.GoldSmall` (150 coins)
- `buy-coins-medium` → `ProductKeyType.GoldMedium` (450 coins)
- `buy-coins-large` → `ProductKeyType.GoldBig` (1000 coins)
- `buy-coins-free` → `ProductKeyType.CoinsFreePack` (100 coins - 24h cooldown)
- `buy-coins-ad` → `ProductKeyType.CoinsAdPack` (100 coins - Watch AD)

## 📱 Features

### Luna UI Store Features

✅ **Real-time Price Updates**
- Tự động lấy giá từ IAPManager
- Cập nhật khi IAP khởi tạo
- Hiển thị local currency

✅ **Currency Integration**
- Hiển thị coins realtime
- Tự động cập nhật sau purchase
- Tích hợp với CurrencyController

✅ **Purchase Handling**
- Haptic feedback
- Audio feedback
- Error handling với SystemMessage
- Success/failure callbacks

✅ **Modern UI**
- UI Toolkit (UIElements) based
- Fade animations
- Responsive design

✅ **Coin Packs System** 🆕
- **5 Coin Pack Types:**
  - Pack 1: 150 coins (IAP)
  - Pack 2: 450 coins (IAP) + "BEST VALUE" badge
  - Pack 3: 1000 coins (IAP)
  - Pack 4: 100 coins (FREE) - 24h cooldown
  - Pack 5: 100 coins (WATCH AD) - Rewarded video
- **Grid Layout** với color-coded packs
- **Smart Cooldown System** cho Free pack
- **AD Integration** với AdsManager
- **Real-time Price Updates** cho IAP packs

### Legacy Store Support

✅ **Backward Compatibility**
- Giữ nguyên UIStore functionality
- Không ảnh hưởng code cũ
- Dễ dàng fallback

## 🎮 Developer Tools

### UIStoreSwitcher

Developer panel để test:
- Switch between stores
- Show/Hide controls
- Real-time status display
- Context menu actions

### IAPStoreIntegrationDemo

Demo functionality:
- Store comparison info
- Purchase event monitoring
- Currency testing
- Integration showcase

## 🪙 Coin Packs System Details

### Pack Types & Rewards

```csharp
// IAP Packs
ProductKeyType.GoldSmall    → 150 coins  (Paid)
ProductKeyType.GoldMedium   → 450 coins  (Paid) 
ProductKeyType.GoldBig      → 1000 coins (Paid)

// Special Packs
ProductKeyType.CoinsFreePack → 100 coins (Free, 24h cooldown)
ProductKeyType.CoinsAdPack   → 100 coins (Watch AD reward)
```

### Free Pack Cooldown System

```csharp
// Constants
private const string FREE_PACK_COOLDOWN_KEY = "free_pack_cooldown";
private const float FREE_PACK_COOLDOWN_HOURS = 24f;

// Usage
float lastClaimTime = PlayerPrefs.GetFloat(FREE_PACK_COOLDOWN_KEY, 0f);
bool canClaim = (Time.time - lastClaimTime) >= (24 * 3600f);
```

### AD Pack Integration

```csharp
// Show rewarded video
AdsManager.ShowRewardBasedVideo(success =>
{
    if (success)
    {
        CurrencyController.Add(CurrencyType.Coins, 100);
        Debug.Log("AD reward coins granted: 100");
    }
});
```

### Visual Design

**Color Coding:**
- 🔵 Small Pack: Blue (`rgba(52, 152, 219, 0.8)`)
- 🟣 Medium Pack: Purple (`rgba(142, 68, 173, 0.8)`)
- 🔴 Large Pack: Red (`rgba(231, 76, 60, 0.8)`)
- 🟢 Free Pack: Green (`rgba(46, 204, 113, 0.8)`)
- 🟡 AD Pack: Yellow (`rgba(241, 196, 15, 0.8)`)

## 🔄 Migration Guide

### Phase 1: Setup (Hiện tại)
- [x] Luna UI Store implementation
- [x] UIStoreManager cho dual system
- [x] Developer tools
- [x] Testing infrastructure
- [x] Coin Packs System 🆕

### Phase 2: Gradual Migration
- [ ] Update store opening calls to use UIStoreManager
- [ ] A/B testing implementation
- [ ] Performance comparison
- [ ] User feedback collection
- [ ] Coin packs analytics

### Phase 3: Full Migration
- [ ] Switch default to Luna UI
- [ ] Remove Legacy Store (optional)
- [ ] Cleanup unused code

## 📊 Performance Comparison

| Feature | Legacy Store | Luna UI Store |
|---------|-------------|---------------|
| Rendering | uGUI | UI Toolkit |
| Performance | Good | Better |
| Styling | Inspector | USS/Inline |
| Animations | Tween | Fade System |
| Responsiveness | Fixed | Flexible |
| Memory | Higher | Lower |
| Coin Packs | ❌ Not supported | ✅ Full support |
| AD Integration | Manual | Automated |
| Cooldown System | ❌ None | ✅ Built-in |

## 🐛 Troubleshooting

### Common Issues

**1. Price không hiển thị**
```csharp
// Check IAP Manager initialization
if (!IAPManager.IsInitialized) {
    Debug.Log("IAP Manager not ready");
}

// Check product data
var productData = IAPManager.GetProductData(ProductKeyType.GoldSmall);
if (productData == null) {
    Debug.Log("Product data not found");
}
```

**2. Store không mở**
```csharp
// Check store type
var currentType = UIStoreManager.GetCurrentStoreType();
Debug.Log($"Current store: {currentType}");

// Check component existence
IAPStoreView storeView = FindObjectOfType<IAPStoreView>();
if (storeView == null) {
    Debug.Log("IAPStoreView not found in scene");
}
```

**3. Purchase không hoạt động**
```csharp
// Check Monetization.IsActive
if (!Monetization.IsActive) {
    Debug.Log("Monetization disabled");
}

// Check product configuration in Monetization Settings
```

**4. Free Pack cooldown issues** 🆕
```csharp
// Check cooldown time
float lastClaim = PlayerPrefs.GetFloat("free_pack_cooldown", 0f);
float timeSince = Time.time - lastClaim;
Debug.Log($"Time since last claim: {timeSince / 3600f} hours");

// Reset cooldown (for testing)
PlayerPrefs.DeleteKey("free_pack_cooldown");
```

**5. AD Pack không hoạt động** 🆕
```csharp
// Check AdsManager state
if (!AdsManager.IsInitialized) {
    Debug.Log("AdsManager not initialized");
}

// Check rewarded video availability
bool hasRewardedVideo = AdsManager.IsRewardBasedVideoAvailable();
Debug.Log($"Rewarded video available: {hasRewardedVideo}");
```

## 🎯 Best Practices

### 1. Store Type Management
- Sử dụng UIStoreManager thay vì direct calls
- Set store type tại app startup
- Cache store preference trong PlayerPrefs

### 2. Error Handling
- Luôn check IAPManager.IsInitialized
- Handle purchase failures gracefully
- Provide user feedback

### 3. Performance
- Sử dụng Luna UI cho better performance
- Cache product data khi có thể
- Minimize FindObjectOfType calls

### 4. Testing
- Test cả hai stores thoroughly
- Verify IAP integration
- Check different screen sizes
- Test edge cases (no internet, etc.)

### 5. Coin Packs Best Practices 🆕
- **Free Pack**: Test cooldown system thoroughly
- **AD Pack**: Handle ad failure gracefully
- **IAP Packs**: Always validate purchase completion
- **UI Feedback**: Show clear success/failure states
- **Analytics**: Track pack popularity và conversion rates

## 📞 Integration Points

### With Existing Systems

**CurrencyController**
```csharp
// Get currency
int coins = CurrencyController.Get(CurrencyType.Coins);

// Add currency
CurrencyController.Add(CurrencyType.Coins, amount);
```

**IAPManager**
```csharp
// Subscribe to events
IAPManager.PurchaseCompleted += OnPurchaseCompleted;
IAPManager.PurchaseFailed += OnPurchaseFailed;

// Get product data
ProductData product = IAPManager.GetProductData(ProductKeyType.GoldSmall);
```

**AdsManager** 🆕
```csharp
// Show rewarded video
AdsManager.ShowRewardBasedVideo(success => {
    if (success) {
        // Grant reward
        CurrencyController.Add(CurrencyType.Coins, 100);
    }
});
```

**AudioController & Haptic**
```csharp
#if MODULE_HAPTIC
Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
AudioController.PlaySound(AudioController.AudioClips.buttonSound);
```

---

## 📝 Technical Debt & Risks

### 1. Rủi Ro Lớn Nhất
- **Complexity**: Maintain 2 hệ thống store song song
- **Testing burden**: Phải test cả 2 stores + coin packs system
- **Code duplication**: Một số logic có thể bị duplicate
- **AD Dependency**: Phụ thuộc vào AdsManager stability 🆕

### 2. Technical Debt
- **FindObjectOfType usage**: Có thể slow, nên cache hoặc dùng singleton
- **UI Toolkit learning curve**: Team cần học UI Toolkit
- **Dependency management**: Luna UI phụ thuộc nhiều systems
- **PlayerPrefs usage**: Free pack cooldown dùng PlayerPrefs (không secure) 🆕
- **Hard-coded values**: Coin amounts và cooldown times được hard-code 🆕

### 3. Coin Packs Specific Risks 🆕
- **Free Pack Exploitation**: User có thể manipulate system time
- **AD Revenue Dependency**: AD pack phụ thuộc vào ad network availability
- **Price Balance**: Coin pack pricing cần continuous balancing
- **Storage**: PlayerPrefs có thể bị clear, mất cooldown data

### Recommendation: 
- **Short term**: Keep dual system for safety
- **Long term**: Migrate hoàn toàn sang Luna UI khi stable
- **Monitoring**: Implement analytics để compare performance
- **Security**: Implement server-side validation cho free pack cooldowns 🆕
- **Flexibility**: Make coin amounts configurable via ScriptableObject 🆕

---

*Created for Bus Stop Jam project - Luna UI Integration v2.0 with Coin Packs System* 