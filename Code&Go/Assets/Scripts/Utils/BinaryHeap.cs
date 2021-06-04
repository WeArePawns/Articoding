using System;
using System.Collections.Generic;

/// <summary>
/// Una montículo binario, estructura de datos útil para organizar data o implementar colas de prioridad.
/// </summary>
/// <typeparam name="T"><![CDATA[IComparable<T> type of item in the heap]]>.</typeparam>
public class BinaryHeap<T> : ICollection<T> where T : IComparable<T>
{
    // Constants
    private const int DEFAULT_SIZE = 4;
    // Fields
    private T[] _data = new T[DEFAULT_SIZE];
    private int _count = 0;
    private int _capacity = DEFAULT_SIZE;
    private bool _sorted;

    // Properties
    /// <summary>
    /// Devuelve el número de valores que hay en el montículo.
    /// </summary>
    public int Count
    {
        get { return _count; }
    }

    /// <summary>
    /// Devuelve o establece la capacidad del montículo
    /// </summary>
    public int Capacity
    {
        get { return _capacity; }
        set
        {
            int previousCapacity = _capacity;
            _capacity = Math.Max(value, _count);
            if (_capacity != previousCapacity)
            {
                T[] temp = new T[_capacity];
                Array.Copy(_data, temp, _count);
                _data = temp;
            }
        }
    }

    // Methods
    /// <summary>
    /// Create un nuevo montículo binario.
    /// </summary>
    public BinaryHeap()
    {
    }
    protected BinaryHeap(T[] data, int count)
    {
        Capacity = count;
        _count = count;
        Array.Copy(data, _data, count);
    }

    /// <summary>
    /// Devuelve el primer valor del montículo sin eliminarlo
    /// </summary>
    /// <returns>El valor más bajo del tipo TValue.</returns>
    public T Peek()
    {
        return _data[0];
    }

    /// <summary>
    /// Elimina todos los elementos del montículo.
    /// </summary>
    public void Clear()
    {
        this._count = 0;
        _data = new T[_capacity];
    }
    /// <summary>
    /// Añade una clave y un valor para el montículo
    /// </summary>
    /// <param name="item">El elemento a añadir al montículo.</param>
    public void Add(T item)
    {
        if (_count == _capacity)
        {
            Capacity *= 2;
        }
        _data[_count] = item;
        UpHeap();
        _count++;
    }

    /// <summary>
    /// Elimina y devuelve el primer elemento del montículo.
    /// </summary>
    /// <returns>El siguiente valor en el montículo.</returns>
    public T Remove()
    {
        if (this._count == 0)
        {
            throw new InvalidOperationException("Cannot remove item, heap is empty.");
        }
        T v = _data[0];
        _count--;
        _data[0] = _data[_count];
        _data[_count] = default(T); //Limpia el último nodo
        DownHeap();
        return v;
    }

    /// <summary>
    /// //////////// CÓDIGO MÁS ESPECÍFICO
    /// </summary>
    /// 

    // Función auxiliar que realiza una inserción up-heap bubbling en el montículo
    private void UpHeap()
    {
        _sorted = false;
        int p = _count;
        T item = _data[p];
        int par = Parent(p);
        while (par > -1 && item.CompareTo(_data[par]) < 0)
        {
            _data[p] = _data[par]; //Swap nodes
            p = par;
            par = Parent(p);
        }
        _data[p] = item;
    }

    // Función auxiliar que realiza una inserción down-heap bubbling en el montículo
    private void DownHeap()
    {
        _sorted = false;
        int n;
        int p = 0;
        T item = _data[p];
        while (true)
        {
            int ch1 = Child1(p);
            if (ch1 >= _count) break;
            int ch2 = Child2(p);
            if (ch2 >= _count)
            {
                n = ch1;
            }
            else
            {
                n = _data[ch1].CompareTo(_data[ch2]) < 0 ? ch1 : ch2;
            }
            if (item.CompareTo(_data[n]) > 0)
            {
                _data[p] = _data[n]; //Swap nodes
                p = n;
            }
            else
            {
                break;
            }
        }
        _data[p] = item;
    }
    private void EnsureSort()
    {
        if (_sorted) return;
        Array.Sort(_data, 0, _count);
        _sorted = true;
    }
    private static int Parent(int index)
    //Función auxiliar que calcula el padre de un nodo
    {
        return (index - 1) >> 1;
    }
    private static int Child1(int index)
    //Función auxiliar que calcula el primero hijo de un nodo
    {
        return (index << 1) + 1;
    }
    private static int Child2(int index)
    //Función auxiliar que calcula el segundo hijo de un nodo
    {
        return (index << 1) + 2;
    }

    /// <summary>
    /// Crea un ejemplar de un montículo binario idéntico a este
    /// </summary>
    /// <returns>Un montículo binario.</returns>
    public BinaryHeap<T> Copy()
    {
        return new BinaryHeap<T>(_data, _count);
    }

    /// <summary>
    /// Devuelve un enumerador para el montículo binario.
    /// </summary>
    /// <returns>Un IEnumerator de tipo T.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        EnsureSort();
        for (int i = 0; i < _count; i++)
        {
            yield return _data[i];
        }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Comprueba para ver si el montículo binario contiene el elemento específico.
    /// </summary>
    /// <param name="item">El elemento por el que buscar en el montículo binario.</param>
    /// <returns>Un booleano, cierto si el montículo binario contiene un elemento.</returns>
    public bool Contains(T item)
    {
        EnsureSort();
        return Array.BinarySearch<T>(_data, 0, _count, item) >= 0;
    }
    /// <summary>
    /// Copia el montículo binario a un array en el índice especificado.
    /// </summary>
    /// <param name="array">Un array unidimensional que es el destino de los elementos copiados.</param>
    /// <param name="arrayIndex">El índice basado en cero en donde comienza la copia.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        EnsureSort();
        Array.Copy(_data, array, _count);
    }
    /// <summary>
    /// Devuelve si es o no de sólo lectura el montínculo binario.
    /// </summary>
    public bool IsReadOnly
    {
        get { return false; }
    }
    /// <summary>
    /// Quita un elemento del montículo binario. Esto utiliza el Comparador de tipo T y no se eliminan duplicados.
    /// </summary>
    /// <param name="item">The item to be removed.</param>
    /// <returns>Boolean true if the item was removed.</returns>
    public bool Remove(T item)
    {
        EnsureSort();
        int i = Array.BinarySearch<T>(_data, 0, _count, item);
        if (i < 0) return false;
        Array.Copy(_data, i + 1, _data, i, _count - i - 1);
        _data[_count - 1] = default(T);
        _count--;
        return true;
    }

    public List<T> ToList()
    {
        T[] arr = new T[_count];
        Array.Copy(_data, arr, _count);
        return new List<T>(arr);
    }
}
