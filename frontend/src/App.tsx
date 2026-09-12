import React, { useEffect, useState } from 'react';
import { Product } from './types/product';
import { Package, Plus, Minus, Trash2, RefreshCw, AlertCircle, CheckCircle2, Layers } from 'lucide-react';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1';

export const App: React.FC = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Formulario nuevo producto
  const [sku, setSku] = useState('');
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState<number>(99.99);
  const [initialStock, setInitialStock] = useState<number>(10);

  const fetchProducts = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`${API_BASE_URL}/products`);
      if (!response.ok) {
        throw new Error(`Error HTTP: ${response.status} - ${response.statusText}`);
      }
      const data = await response.json();
      setProducts(data);
    } catch (err: any) {
      setError(err.message || 'No se pudo conectar con la API.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const handleAdjustStock = async (id: string, delta: number) => {
    setError(null);
    setSuccess(null);
    try {
      const response = await fetch(`${API_BASE_URL}/products/${id}/stock`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ delta }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail || 'Fallo al ajustar el stock.');
      }

      setSuccess(`Stock actualizado exitosamente (${delta > 0 ? `+${delta}` : delta} unidades).`);
      await fetchProducts();
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('¿Estás seguro de eliminar este producto del catálogo?')) return;
    setError(null);
    setSuccess(null);
    try {
      const response = await fetch(`${API_BASE_URL}/products/${id}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        throw new Error('No se pudo eliminar el producto.');
      }

      setSuccess('Producto eliminado correctamente.');
      await fetchProducts();
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    try {
      const response = await fetch(`${API_BASE_URL}/products`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          sku,
          name,
          description: description || null,
          price: Number(price),
          initialStock: Number(initialStock),
        }),
      });

      if (!response.ok) {
        const errData = await response.json();
        throw new Error(errData.detail || 'Error al registrar el producto.');
      }

      setSuccess(`Producto '${name}' creado exitosamente.`);
      setSku('');
      setName('');
      setDescription('');
      setPrice(99.99);
      setInitialStock(10);
      await fetchProducts();
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <header className="mb-8 border-b border-slate-800 pb-6 flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-indigo-600 rounded-lg shadow-lg">
                <Layers className="w-7 h-7 text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold tracking-tight text-white sm:text-3xl">
                  Plataforma de Catálogo e Inventario
                </h1>
                <p className="text-sm text-slate-400">
                  Arquitectura Limpia (.NET 8 + PostgreSQL 16 + React 18) &bull; Universidad Popular del Cesar
                </p>
              </div>
            </div>
          </div>
          <button
            onClick={fetchProducts}
            disabled={loading}
            className="inline-flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-200 text-sm font-medium rounded-md transition shadow-sm"
          >
            <RefreshCw className={`w-4 h-4 ${loading ? 'animate-spin' : ''}`} />
            Actualizar Catálogo
          </button>
        </header>

        {/* Notificaciones */}
        {error && (
          <div className="mb-6 p-4 bg-rose-950/80 border border-rose-800 rounded-lg flex items-center gap-3 text-rose-200">
            <AlertCircle className="w-5 h-5 flex-shrink-0 text-rose-400" />
            <p className="text-sm">{error}</p>
          </div>
        )}

        {success && (
          <div className="mb-6 p-4 bg-emerald-950/80 border border-emerald-800 rounded-lg flex items-center gap-3 text-emerald-200">
            <CheckCircle2 className="w-5 h-5 flex-shrink-0 text-emerald-400" />
            <p className="text-sm">{success}</p>
          </div>
        )}

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Formulario de Creación */}
          <div className="lg:col-span-1">
            <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-xl sticky top-8">
              <h2 className="text-lg font-semibold text-white mb-4 flex items-center gap-2">
                <Package className="w-5 h-5 text-indigo-400" />
                Registrar Nuevo Producto
              </h2>
              <form onSubmit={handleCreate} className="space-y-4">
                <div>
                  <label className="block text-xs font-medium text-slate-400 mb-1">SKU (Código Único)</label>
                  <input
                    type="text"
                    required
                    value={sku}
                    onChange={(e) => setSku(e.target.value)}
                    placeholder="Ej. MON-LG-27"
                    className="w-full bg-slate-950 border border-slate-700 rounded-md px-3 py-2 text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-slate-400 mb-1">Nombre Comercial</label>
                  <input
                    type="text"
                    required
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    placeholder="Nombre del producto"
                    className="w-full bg-slate-950 border border-slate-700 rounded-md px-3 py-2 text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-slate-400 mb-1">Descripción</label>
                  <textarea
                    rows={2}
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Detalles y especificaciones"
                    className="w-full bg-slate-950 border border-slate-700 rounded-md px-3 py-2 text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                  />
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Precio ($)</label>
                    <input
                      type="number"
                      step="0.01"
                      min="0.01"
                      required
                      value={price}
                      onChange={(e) => setPrice(parseFloat(e.target.value) || 0)}
                      className="w-full bg-slate-950 border border-slate-700 rounded-md px-3 py-2 text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-medium text-slate-400 mb-1">Stock Inicial</label>
                    <input
                      type="number"
                      min="0"
                      required
                      value={initialStock}
                      onChange={(e) => setInitialStock(parseInt(e.target.value, 10) || 0)}
                      className="w-full bg-slate-950 border border-slate-700 rounded-md px-3 py-2 text-sm text-slate-100 focus:outline-none focus:border-indigo-500"
                    />
                  </div>
                </div>
                <button
                  type="submit"
                  className="w-full py-2.5 px-4 bg-indigo-600 hover:bg-indigo-500 text-white font-medium text-sm rounded-md shadow transition"
                >
                  Guardar en Catálogo
                </button>
              </form>
            </div>
          </div>

          {/* Listado de Productos */}
          <div className="lg:col-span-2">
            <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-xl">
              <h2 className="text-lg font-semibold text-white mb-4 flex items-center justify-between">
                <span>Catálogo en Existencia</span>
                <span className="text-xs bg-slate-800 text-slate-400 px-2.5 py-1 rounded-full">
                  {products.length} productos
                </span>
              </h2>

              {loading && products.length === 0 ? (
                <div className="py-12 text-center text-slate-500 text-sm">
                  Cargando catálogo desde WebApi...
                </div>
              ) : products.length === 0 ? (
                <div className="py-12 text-center text-slate-500 text-sm">
                  No hay productos registrados en el catálogo.
                </div>
              ) : (
                <div className="space-y-4">
                  {products.map((product) => (
                    <div
                      key={product.id}
                      className="p-4 bg-slate-950 border border-slate-800/80 rounded-lg hover:border-slate-700 transition flex flex-col sm:flex-row sm:items-center justify-between gap-4"
                    >
                      <div className="space-y-1">
                        <div className="flex items-center gap-2">
                          <span className="text-xs font-mono px-2 py-0.5 bg-indigo-950/60 text-indigo-400 border border-indigo-900 rounded">
                            {product.sku}
                          </span>
                          <h3 className="font-medium text-slate-200">{product.name}</h3>
                        </div>
                        {product.description && (
                          <p className="text-xs text-slate-400 max-w-md">{product.description}</p>
                        )}
                        <div className="text-xs text-slate-500">
                          Precio: <span className="text-emerald-400 font-semibold">${product.price.toFixed(2)}</span>
                        </div>
                      </div>

                      {/* Control de Stock */}
                      <div className="flex items-center gap-4">
                        <div className="text-right">
                          <span className="block text-xs text-slate-400">Stock</span>
                          <span
                            className={`text-base font-bold ${
                              product.stock === 0
                                ? 'text-rose-400'
                                : product.stock < 10
                                ? 'text-amber-400'
                                : 'text-slate-200'
                            }`}
                          >
                            {product.stock} u.
                          </span>
                        </div>
                        <div className="flex items-center gap-1 bg-slate-900 border border-slate-800 rounded-md p-1">
                          <button
                            title="Reducir stock (-1)"
                            onClick={() => handleAdjustStock(product.id, -1)}
                            className="p-1 text-slate-400 hover:text-white hover:bg-slate-800 rounded transition"
                          >
                            <Minus className="w-4 h-4" />
                          </button>
                          <button
                            title="Aumentar stock (+1)"
                            onClick={() => handleAdjustStock(product.id, 1)}
                            className="p-1 text-slate-400 hover:text-white hover:bg-slate-800 rounded transition"
                          >
                            <Plus className="w-4 h-4" />
                          </button>
                        </div>
                        <button
                          title="Eliminar producto"
                          onClick={() => handleDelete(product.id)}
                          className="p-2 text-slate-500 hover:text-rose-400 transition"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default App;
