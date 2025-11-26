import { BrowserRouter } from 'react-router-dom';
import { AppRouter } from './presentation/router/AppRouter';

function App() {
  return (
    <BrowserRouter>
      <AppRouter />
    </BrowserRouter>
  );
}

export default App;