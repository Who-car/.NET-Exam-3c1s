import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';

const PrivateRoute = () => {
  const token = localStorage.getItem('token')
  // TODO: изменить обратно, когда сервак встанет
  if (token) {
    return <Outlet/>
  } else {
    return <Navigate to="/sign-in" replace />;
  }
};

export default PrivateRoute;