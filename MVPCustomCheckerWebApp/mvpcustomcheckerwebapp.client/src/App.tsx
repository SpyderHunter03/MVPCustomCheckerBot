import { BrowserRouter, Routes, Route } from "react-router-dom";
import LandingPage from './pages/LandingPage';
import LoginPage from './pages/LoginPage';
import AvailableMoldsPage from './pages/AvailableMoldsPage';
import PreviousFilesPage from "./pages/PreviousFilesPage";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";

function App() {
  const queryClient = new QueryClient()

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/available-molds" element={<AvailableMoldsPage />} />
          <Route path="/previous-files" element={<PreviousFilesPage />} />
        </Routes>
      </BrowserRouter>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
      
    );
}

export default App;