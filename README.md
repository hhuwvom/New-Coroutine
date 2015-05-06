NewCoroutine
============

Let's kick out unity's bad coroutine. Please free to use.

Sample in /Assets/Scripts/Coroutine/Sample

## How to use

```c#
IEnumator Foo() {
    yield return null;
}
```

### Simple coroutine

```c#
CoroutineExecutor.Do(Foo());
```

### Group coroutine

```c#
var group = new GroupCoroutine(Foo());
group.Add(Foo());
CoroutineExecutor.Do(group);
```

### Order courtine

```c#
var order = new OrderCoroutine(Foo());
order.Add(Foo());
CoroutineExecutor.Do(order);
```

### Event coroutine

```c#

// It will wait until receiving event
var ev = new EventCoroutine(new string[] {"ABC", "123"});
yield return ev;

// After some coroutine wait for event,  You can send event by this way to trigger EventCoroutine to finish. 
CoroutineExeCcutor.SendEvent("ABC");

// Then every EventCoroutine wait for "ABC" event will be finish.
```

### Executing during FixedUpdate()

```c#
yield return BaseCoroutine.WaitFor.FixedUpdate;
or yield return new WaitForFixedUpdate();
```

### Excuting during EndOfFrame()

```c#
yield return BaseCoroutine.WaitFor.EndOfFrame;
or yield return new WaitForEndOfFrame();
```
