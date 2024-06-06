# BlahEditor
The purpose of this Unity package is to provide:
* A set of attributes and drawers for them in Odin-like manner.
* SubAssets mechanism for complex game configs.
* Other Editor utilities.

## SubAssets
SubAssets is a way to create one Scriptable Object inside another.
This might be useful in a situation when you have a factory-like configs. 

For instance a game has quests that should have a trigger and a reward. There might be a dozen of different triggers and rewards, so you want to have a way to mix them. In classic pipeline you will probably have quest SO that have a reference to a trigger SO and to reward SO. So, there are already three files, and things will start getting messy once you have a hundred of such quests (300 files in total).

SubAssets allows you to create trigger SO and reward SO inside your quest SO, so the parentship will be explicit.

In fact, you can use SerializedReference for that purpose, however it seems to be unstable in some cases which is the main motivation of using SubAssets.
```csharp
// Create a new property attribute.
public class RewardAttribute : PropertyAttribute {}

// Create a new propety drawer inherited from SubAssetDrawer.
[CustomPropertyDrawer(typeof(RewardAttribute))]
public class RewardDrawer : SubAssetDrawer
{
	protected override IReadOnlyList<Type> Types { get; } = new Type[]
    {
        // List all possible sub assets types.
        typeof(RewardMoney),
        typeof(RewardCookie)
    };
}

// Create parent base SO class.
public abstract class RewardBase : ScriptableObject {}

// Create different children of base class.
public class RewardMoney : MySubAssetBase 
{
    public int Amount;
}
public class RewardCookie : MySubAssetBase
{
    public bool IsChocolate;
    public int Count;
}

public class Task : ScriptableObject
{
    // Put your attribute on the field of base class.
    // You can create a new sub asset of any children type in Editor (using "add" button).
    // You can optionaly place InlineSO attribute to edit SubAsset in place.
    [InlineSO(true), Reward]
    public Rewardbase Reward;

    public bool TryGetRewardInCookies(out RewardCookie reward)
    {
        if (Reward is RewardCookie cookie)
        {
            reward = cookie;
            return true;
        }
        return false;
    }
}
```


## Attributes
### InlineSO
```csharp
// Draw Scriptable Object content in place, read-only.
[InlineSO(false)]
public SomeAsset Asset;

// Draw Scriptable Object content in place.
[InlineSO(true)]
public SomeAsset Asset;

class SomeAsset : ScriptableObject
{
    public int X;
}
```

### NoFoldout
```csharp
// The class Desc is drawn without collapse/expand spoiler.
[NoFoldout]
public Desc Inner;

class Desc
{
    public int X;
}
```

### ArrayByEnum
```csharp
// Array length equals to count of Enum values, 
// labels of elements replaced by names of Enum values.
[ArrayByEnum(typeof(EItem))]
public int[] Costs;

enum EItem
{
    ItemA,
    ItemB,
    ItemC
}
```

### Button
```csharp
// Field is drawn as a button with specified name. Click on the button invokes the specified method.
[SerializedField, Button("Button Name", nameof(OnEditorButtonTap))]
private bool _editorButton;

private void OnEditorButtonTap() {}
```

### Disabled
```csharp
// Field is drawn in read-only mode.
[Disabled]
public int NonEditableField;
```

### Info
```csharp
[Info("Draw help box above the field.")]
public int X;
```

### ShowIf
```csharp
public bool UseX;

// Draw field only if specified predicate field (or property) is true.
[ShowIf(nameof(UseX))]
public int X;
```

### TableArray
```csharp
// Draw each array element horizontally.
[TableArray]
public Pair[] Pairs;

// Draw each array element horizontally with specified width ratio. 
[TableArray(0.3f, 0.7f)]
public Pair[] Pairs;

public class Pair
{
    public int A;
    public int B;
}
```

## SafeEnum
SafeEnum purpose is to serialize any enum as a string and parse it later. This prevents losing of data in cases when enum entries change order or explicit values.
```csharp
[SerializedField]
private SafeEnum<EType> _savedType;

EType type;
if (_savedType.IsValid)
{
    type = _savedType.Val;
}
else
{
    type = EType.Unknown;
    Debug.Warn($"failed to parse {_savedType.BackingStr}");
}

enum EType
{
    Unknown,
    TypeA,
    TypeB
}
```