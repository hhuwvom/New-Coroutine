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
CoroutineExcutor.Do(Foo());
```

### Group coroutine

```c#
GroupCoroutine group = new GroupCoroutine(Foo());
group.Add(Foo());
CoroutineExcutor.Do(group);
```

### Order courtine

```c#
OrderCoroutine order = new OrderCoroutine(Foo());
order.Add(Foo());
CoroutineExcutor.Do(order);
```
