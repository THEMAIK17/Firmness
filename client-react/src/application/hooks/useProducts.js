import { useState, useEffect } from 'react';
import { productRepository } from '../../infraestructure/repositories/productRepository';

export const useProducts = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const loadProducts = async () => {
            try {
                setLoading(true);
                const data = await productRepository.getAll();
                setProducts(data);
            } catch (err) {
                setError('No se pudieron cargar los productos.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        loadProducts();
    }, []);

    return { products, loading, error };
};