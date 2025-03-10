import axios from 'axios';
import { API_BASE_URL } from './config';
import { toast } from 'react-toastify';

const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.status === 401 && localStorage.getItem('token')) localStorage.removeItem('token')
    const message = error.response.data.Message || error.message || error
    toast.error(`Unknown error: ${message}`)
    return Promise.reject(error);
  }
);

export default axiosInstance;