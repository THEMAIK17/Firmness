import { createContext, useContext, useState, useEffect } from 'react';

const CartContext = createContext();

export const CartProvider = ({ children }) => {
    // Iniciamos el carrito leyendo del localStorage (Persistencia)
    const [cartItems, setCartItems] = useState(() => {
        const savedCart = localStorage.getItem('cart');
        return savedCart ? JSON.parse(savedCart) : [];
    });

    // Guardar en localStorage cada vez que cambie el carrito
    useEffect(() => {
        localStorage.setItem('cart', JSON.stringify(cartItems));
    }, [cartItems]);

    // 1. Agregar Producto
    const addToCart = (product) => {
        setCartItems(prev => {
            const exists = prev.find(item => item.id === product.id);
            if (exists) {
                // Si ya existe, aumentamos cantidad
                return prev.map(item => 
                    item.id === product.id 
                        ? { ...item, quantity: item.quantity + 1 } 
                        : item
                );
            }
            // Si es nuevo, lo agregamos con cantidad 1
            return [...prev, { ...product, quantity: 1 }];
        });
    };

    // 2. Remover Producto
    const removeFromCart = (productId) => {
        setCartItems(prev => prev.filter(item => item.id !== productId));
    };

    // 3. Limpiar Carrito (Para cuando compre)
    const clearCart = () => setCartItems([]);

    // 4. Cálculos (Subtotal, IVA, Total)
    const calculateTotals = () => {
        const subtotal = cartItems.reduce((acc, item) => acc + (item.unitPrice * item.quantity), 0);
        const tax = subtotal * 0.19; // IVA del 19%
        const total = subtotal + tax;
        return { subtotal, tax, total };
    };

    return (
        <CartContext.Provider value={{ 
            cartItems, 
            addToCart, 
            removeFromCart, 
            clearCart,
            calculateTotals 
        }}>
            {children}
        </CartContext.Provider>
    );
};

// Hook para usar el carrito
export const useCart = () => useContext(CartContext);