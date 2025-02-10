import './App.css'
import {AuthProvider} from "./AuthProvider.tsx";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import {StartPage} from "./StartPage.tsx";
import {ProtectedRoute} from "./ProtectedRoute.tsx";
import {Register} from "./Register.tsx";
import {GamesList} from "./GamesList.tsx";
import {Game} from "./Game.tsx";

function App() {

  return (
    <>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<StartPage />} />
            <Route path="/register" element={<Register />} />
            <Route path="/games" element={<ProtectedRoute element={<GamesList />} />} />
            <Route path="/main" element={<ProtectedRoute element={<Game />} />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </>
  )
}

export default App
