import axiosInstance from './axiosInstance';
import { toast } from 'react-toastify';

export const getToken = () => {
  return localStorage.getItem('token')
}

export const signUp = async (userData) => {
  try {
    const response = await axiosInstance.post('/auth/signup', userData);
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
    }
    return response.data;
  } catch (error) {
    toast.error(`error [sign up]: ${error.message || error}`)
    throw error;
  }
};

export const signIn = async (credentials) => {
  try {
    const response = await axiosInstance.post('/auth/signin', credentials);
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
    }
    return response.data;
  } catch (error) {
    toast.error(`error [sign in]: ${error.message || error}`)
    throw error;
  }
};

export const signOut = async () => {
  try {
    localStorage.removeItem('token');
    return response.data;
  } catch (error) {
    toast.error(`error [sign out]: ${error.message || error}`)
    throw error;
  }
};
