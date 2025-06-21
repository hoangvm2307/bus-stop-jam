# Gameplay UI - Luna UI Implementation

Gameplay UI được tạo theo thiết kế trong ảnh với layout chính xác và tích hợp với các systems hiện có.

## 🎨 UI Layout

```
┌─────────────────────────────────────────────────────────────┐
│  [❤️ FULL]      [LEVEL 2]      [🪙 832K]                   │
│                                                 [🪙]         │
│                                                 [🎭]         │
│                                                 [🚫]         │
│                                                             │
│              GAMEPLAY AREA                                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## 📁 File Structure

```
Assets/Project Files/Game/Scripts/LunaUI/
├── Views/Screens/
│   ├── GameplayUIStyle.uss      # CSS Styling
│   └── GameplayUIView.uxml      # UXML Layout
├── Presenters/Screens/
│   └── GameplayUIView.cs        # Main Component
└── Demo/
    └── GameplayUIDemo.cs        # Testing Script
```

## 🚀 Quick Setup

### 1. Tạo GameObject mới trong Scene
```
1. Create Empty GameObject: "Gameplay UI"
2. Add UIDocument component
3. Set Source Asset: GameplayUIView.uxml
4. Add GameplayUIView.cs script
```

### 2. Setup Demo (Optional)
```
1. Add GameplayUIDemo.cs to any GameObject
2. Run scene
3. Use keyboard controls to test
```

## 🎯 Component Features

### GameplayUIView.cs

#### Real-time Updates
- **Coins**: Automatic từ CurrencyController
- **Level**: Từ PlayerPrefs hoặc level system
- **Lives**: Tích hợp với lives system

#### Button Functions
- **Store Button**: Mở Luna UI Store (IAPStoreView)
- **Equipment Button**: Mở Skin Store (UISkinStore)
- **No-Ads Button**: Direct purchase No-Ads

#### Public Methods
```csharp
// Manual controls
gameplayUI.SetLevel(5);
gameplayUI.SetLives(3, 5);
gameplayUI.RefreshUI();
gameplayUI.SetUIVisible(false);
```

## 🎮 Controls & Testing

### Demo Controls (GameplayUIDemo)
- **L**: Next Level
- **C**: Add 100 Coins  
- **H**: Toggle Lives (Full/5)
- **U**: Hide/Show UI
- **R**: Refresh UI

### GUI Buttons
- Add 500 Coins
- Next Level  
- Toggle UI

## 🎨 Styling Features

### CSS Classes
```css
.gameplay-ui          # Root container
.gameplay-hud         # Top bar
.hearts-container     # Lives display
.level-container      # Level display  
.coins-container      # Currency display
.side-buttons         # Right side buttons
.side-button          # Individual button
```

### Button Variants
```css
.side-button--store      # Store button (gold theme)
.side-button--equipment  # Equipment button (purple theme)
.side-button--no-ads     # No-ads button (red theme)
```

### Animations
```css
.bounce-in           # Entry animation
.pulse-animation     # Pulsing effect (for full lives)
```

## 🔧 Integration Points

### Currency System
```csharp
// Auto-updates from CurrencyController
CurrencyController.SubscribeGlobalCallback(OnCurrencyChanged);
```

### Store Systems
```csharp
// Luna UI Store
UIStoreManager.SetStoreType(UIStoreManager.StoreType.LunaUIStore);
UIStoreManager.ShowStore();

// Skin Store
UIController.ShowPage<Watermelon.SkinStore.UISkinStore>();
```

### IAP System
```csharp
// Direct No-Ads purchase
IAPManager.BuyProduct(ProductKeyType.NoAds);
```

## 📱 Mobile Responsive

### Breakpoints
- **Mobile**: < 600px width
- **Desktop**: >= 600px width

### Mobile Adjustments
- Smaller button sizes (60px vs 70px)
- Reduced padding and margins
- Optimized touch targets
- Smaller font sizes

## 🎵 Audio & Haptics

### Feedback Systems
```csharp
// Audio feedback
AudioController.PlaySound(AudioController.AudioClips.buttonSound);

// Haptic feedback (if available)
#if MODULE_HAPTIC
Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif
```

## 🔍 Debugging

### Console Logs
- Button click events
- Currency changes
- Level changes
- Store openings

### Debug Info
```csharp
Debug.Log($"Gained {amount} coins!");
Debug.Log("Store button clicked - Opening IAP Store");
Debug.Log("Equipment button clicked - Opening Skin Store");
```

## ⚙️ Customization

### Colors
```css
/* Heart container - green theme */
border-color: rgba(46, 204, 113, 0.8);
color: rgb(46, 204, 113);

/* Level container - blue theme */
background-color: rgba(52, 152, 219, 0.9);

/* Coins container - gold theme */
border-color: rgba(241, 196, 15, 0.8);
color: rgb(241, 196, 15);
```

### Button Icons
Thay đổi background-image trong CSS:
```css
.side-button--store {
    background-image: url("path/to/your/icon.png");
}
```

## 🐛 Troubleshooting

### Common Issues

1. **UI không hiển thị**
   - Check UXML path trong UIDocument
   - Verify USS file import

2. **Buttons không hoạt động**
   - Check GameplayUIView.cs attached
   - Verify Awake() initialization

3. **Currency không update**
   - Check CurrencyController.SubscribeGlobalCallback
   - Verify OnEnable/OnDisable events

4. **Store không mở**
   - Check UIStoreManager exists
   - Verify IAPManager initialization

### Performance Tips
- Disable auto-update nếu không cần thiết
- Tăng updateInterval để giảm CPU usage
- Use object pooling cho floating effects

## 📊 Technical Debt

### Current Limitations
1. **Lives System**: Placeholder implementation
2. **Level System**: Uses PlayerPrefs instead of proper system
3. **Equipment Store**: Basic integration without customization

### Future Improvements
1. Proper lives system integration
2. Advanced level progression system
3. Customizable equipment store
4. More animation effects
5. Better performance optimization 