import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';

//global styles
import 'bootstrap/dist/css/bootstrap.min.css';

// import AuthProvider to wrap the app
import { AuthProvider } from './application/context/AuthContext';
import { CartProvider } from './application/context/CartContext.jsx';

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <AuthProvider>
      <CartProvider>
      <App />
      </CartProvider>
    </AuthProvider>
  </React.StrictMode>,
);