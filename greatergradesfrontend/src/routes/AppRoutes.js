import Dashboard from "../pages/Dashboard";
import Login from "../pages/Login";
import Register from "../pages/Register";
import { Navigate, Route, Routes } from 'react-router-dom';
import { useContext } from "react";
import { UserContext } from "../functions/UserContext";

const AppRoutes = () => {
    const { authToken } = useContext(UserContext);

    return (
        <Routes>
            <Route path='/' element={!authToken ? <Navigate to='/login' /> : <Navigate to ='/dashboard' />} />
            <Route path='/login' element={!authToken ? <Login /> : <Navigate to='/dashboard' />} />
            <Route path='/dashboard' element={!authToken ? <Navigate to='/login' /> : <Dashboard />} />
            <Route path='/register' element={!authToken ? <Register /> : <Navigate to='/dashboard' />} />
        </Routes >
    )
}

export default AppRoutes;