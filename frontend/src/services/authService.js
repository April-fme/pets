import api from './api';

const TOKEN_KEY = 'pets_auth_token';
const USER_KEY = 'pets_user';

export const authService = {
  async login(username, password) {
    const makeLoginRequest = async (attempt = 1) => {
      try {
        console.log(`嘗試登入 (第 ${attempt} 次):`, { username, password });
        const response = await api.post('/auth/login', { username, password });
        console.log('登入響應:', response.data);
        const { token } = response.data;
        
        if (!token) {
          throw new Error('未收到 token');
        }
        
        localStorage.setItem(TOKEN_KEY, token);
        const user = parseJwt(token);
        localStorage.setItem(USER_KEY, JSON.stringify(user));
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        console.log('登入成功，已設置 token:', token.substring(0, 20) + '...');
        return response.data;
      } catch (error) {
        const errorMessage = error.response?.data || error.message || '';
        const isTransientFailure = errorMessage.includes('transient failure');
        
        console.error(`登入第 ${attempt} 次失敗:`, {
          status: error.response?.status,
          statusText: error.response?.statusText,
          data: error.response?.data,
          message: error.message,
          isTransientFailure
        });
        
        // 如果是 transient failure 且是第一次嘗試，自動重試
        if (isTransientFailure && attempt === 1) {
          console.warn('⚠️ 偵測到伺服器暫時故障，等待 2 秒後進行第二次嘗試...');
          await new Promise(resolve => setTimeout(resolve, 2000));
          return makeLoginRequest(2);
        }
        
        // 第二次仍然失敗或其他錯誤，拋出錯誤
        throw error;
      }
    };
    
    return makeLoginRequest();
  },

  async register(username, email, password, fullName) {
    try {
      console.log('嘗試註冊:', { username, email, fullName });
      const response = await api.post('/auth/register', {
        username,
        email,
        password,
        fullName,
      });
      console.log('註冊響應:', response.data);
      const { token } = response.data;
      localStorage.setItem(TOKEN_KEY, token);
      const user = parseJwt(token);
      localStorage.setItem(USER_KEY, JSON.stringify(user));
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      return response.data;
    } catch (error) {
      console.error('註冊錯誤:', error.response?.data || error.message);
      throw error;
    }
  },

  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    delete api.defaults.headers.common['Authorization'];
  },

  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  },

  getCurrentUser() {
    const userStr = localStorage.getItem(USER_KEY);
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch {
        return null;
      }
    }
    return null;
  },

  isAuthenticated() {
    const token = this.getToken();
    if (!token) return false;
    const user = parseJwt(token);
    if (!user || !user.exp) return false;
    return user.exp * 1000 > Date.now();
  },

  initAuth() {
    const token = this.getToken();
    if (token && this.isAuthenticated()) {
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      console.log('認證已恢復，token:', token.substring(0, 20) + '...');
      return true;
    }
    this.logout();
    return false;
  },
};

function parseJwt(token) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    const parsed = JSON.parse(jsonPayload);
    console.log('解析的 JWT:', parsed);
    return parsed;
  } catch (error) {
    console.error('JWT 解析失敗:', error);
    return null;
  }
}

export default authService;
