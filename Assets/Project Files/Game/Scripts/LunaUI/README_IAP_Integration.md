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
             └── Purchase handling
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
```

### 3. Product Mapping

Luna UI buttons được map với ProductKeyType:
- `buy-no-ads` → `ProductKeyType.NoAds`
- `buy-starter-pack` → `ProductKeyType.StarterPack`
- `buy-power-pack` → `ProductKeyType.PUPack`

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

## 🔄 Migration Guide

### Phase 1: Setup (Hiện tại)
- [x] Luna UI Store implementation
- [x] UIStoreManager cho dual system
- [x] Developer tools
- [x] Testing infrastructure

### Phase 2: Gradual Migration
- [ ] Update store opening calls to use UIStoreManager
- [ ] A/B testing implementation
- [ ] Performance comparison
- [ ] User feedback collection

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

## 🐛 Troubleshooting

### Common Issues

**1. Price không hiển thị**
```csharp
// Check IAP Manager initialization
if (!IAPManager.IsInitialized) {
    Debug.Log("IAP Manager not ready");
}

// Check product data
var productData = IAPManager.GetProductData(ProductKeyType.NoAds);
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
ProductData product = IAPManager.GetProductData(ProductKeyType.NoAds);
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
- **Testing burden**: Phải test cả 2 stores
- **Code duplication**: Một số logic có thể bị duplicate

### 2. Technical Debt
- **FindObjectOfType usage**: Có thể slow, nên cache hoặc dùng singleton
- **UI Toolkit learning curve**: Team cần học UI Toolkit
- **Dependency management**: Luna UI phụ thuộc nhiều systems

### Recommendation: 
- **Short term**: Keep dual system for safety
- **Long term**: Migrate hoàn toàn sang Luna UI khi stable
- **Monitoring**: Implement analytics để compare performance

---

*Created for Bus Stop Jam project - Luna UI Integration* 