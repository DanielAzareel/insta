import axios from 'axios';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5002';

export const api = axios.create({
  baseURL: apiBaseUrl,
  timeout: 15000,
});
