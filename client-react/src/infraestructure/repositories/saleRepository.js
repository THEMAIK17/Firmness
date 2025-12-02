import api from '../api/axiosConfig';

export const saleRepository = {
    // Create sale (POST)
    createSale: async (saleData) => {
        const response = await api.post('/Sales', saleData);
        return response.data;
    },

    // see sales of the logged-in user (GET)
    getMySales: async () => {
       
        const response = await api.get('/Sales/MySales'); 
        return response.data;
    },

    // get sale receipt PDF 
    // I Use 'blob' because the  PDF is binary file
    getReceiptPdf: async (saleId) => {
        const response = await api.get(`/Sales/${saleId}/Pdf`, { 
            responseType: 'blob' 
        });
        return response.data;
    }

};