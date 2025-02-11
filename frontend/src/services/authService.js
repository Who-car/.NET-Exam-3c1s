import axiosInstance from './axiosInstance';
import { toast } from 'react-toastify';

export const getToken = () => {
  return localStorage.getItem('token')
}

export const signUp = async (userData) => {
  try {
    const response = await axiosInstance.post('/auth/signup', userData);
    console.log('response: ', response)
    if (response.data) {
      localStorage.setItem('token', response.data);
    }
    return response.data;
  } catch (error) {
    const message = error.response.data.Message || error.message || error
    toast.error(`error [sign up]: ${message}`)
    throw error;
  }
};

export const signIn = async (credentials) => {
  try {
    const response = await axiosInstance.post('/auth/signin', credentials);
    if (response.data) {
      localStorage.setItem('token', response.data);
    }
    return response.data;
  } catch (error) {
    const message = error.response.data.Message || error.message || error
    toast.error(`error [sign in]: ${message}`)
    throw error;
  }
};

export const signOut = async () => {
  try {
    localStorage.removeItem('token');
    return response.data;
  } catch (error) {
    const message = error.response.data.Message || error.message || error
    toast.error(`error [sign out]: ${message}`)
    throw error;
  }
};
