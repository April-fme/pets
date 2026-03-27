import React, { useState } from 'react';
import authService from '../services/authService';

function LoginPage({ onLoginSuccess }) {
  const [mode, setMode] = useState('login'); // 'login' or 'register'
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    fullName: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setError('');
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    if (!formData.username || !formData.password) {
      setError('請填寫用戶名和密碼');
      return;
    }
    setLoading(true);
    try {
      await authService.login(formData.username, formData.password);
      onLoginSuccess();
    } catch (err) {
      const msg = err.response?.data || err.message;
      setError(typeof msg === 'string' ? msg : '登入失敗，請確認帳號密碼');
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    if (!formData.username || !formData.email || !formData.password) {
      setError('請填寫所有必填欄位');
      return;
    }
    if (formData.password !== formData.confirmPassword) {
      setError('兩次輸入的密碼不一致');
      return;
    }
    if (formData.password.length < 6) {
      setError('密碼至少需要6個字符');
      return;
    }
    setLoading(true);
    try {
      await authService.register(
        formData.username,
        formData.email,
        formData.password,
        formData.fullName || undefined
      );
      onLoginSuccess();
    } catch (err) {
      const msg = err.response?.data || err.message;
      setError(typeof msg === 'string' ? msg : '註冊失敗，請稍後再試');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <div style={styles.header}>
          <h1 style={styles.title}>🐾 寵物健康管理系統</h1>
          <p style={styles.subtitle}>請登入以繼續使用</p>
        </div>

        <div style={styles.tabs}>
          <button
            style={{ ...styles.tab, ...(mode === 'login' ? styles.activeTab : {}) }}
            onClick={() => { setMode('login'); setError(''); }}
          >
            登入
          </button>
          <button
            style={{ ...styles.tab, ...(mode === 'register' ? styles.activeTab : {}) }}
            onClick={() => { setMode('register'); setError(''); }}
          >
            註冊
          </button>
        </div>

        {error && <div style={styles.errorBox}>{error}</div>}

        {mode === 'login' ? (
          <form onSubmit={handleLogin} style={styles.form}>
            <div style={styles.fieldGroup}>
              <label style={styles.label}>用戶名</label>
              <input
                type="text"
                name="username"
                value={formData.username}
                onChange={handleChange}
                style={styles.input}
                placeholder="請輸入用戶名"
                autoComplete="username"
              />
            </div>
            <div style={styles.fieldGroup}>
              <label style={styles.label}>密碼</label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                style={styles.input}
                placeholder="請輸入密碼"
                autoComplete="current-password"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              style={{ ...styles.submitBtn, opacity: loading ? 0.7 : 1 }}
            >
              {loading ? '登入中...' : '登入'}
            </button>
          </form>
        ) : (
          <form onSubmit={handleRegister} style={styles.form}>
            <div style={styles.fieldGroup}>
              <label style={styles.label}>用戶名 <span style={styles.required}>*</span></label>
              <input
                type="text"
                name="username"
                value={formData.username}
                onChange={handleChange}
                style={styles.input}
                placeholder="請輸入用戶名"
                autoComplete="username"
              />
            </div>
            <div style={styles.fieldGroup}>
              <label style={styles.label}>電子郵件 <span style={styles.required}>*</span></label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                style={styles.input}
                placeholder="請輸入電子郵件"
                autoComplete="email"
              />
            </div>
            <div style={styles.fieldGroup}>
              <label style={styles.label}>顯示名稱（選填）</label>
              <input
                type="text"
                name="fullName"
                value={formData.fullName}
                onChange={handleChange}
                style={styles.input}
                placeholder="請輸入顯示名稱"
                autoComplete="name"
              />
            </div>
            <div style={styles.fieldGroup}>
              <label style={styles.label}>密碼 <span style={styles.required}>*</span></label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                style={styles.input}
                placeholder="至少6個字符"
                autoComplete="new-password"
              />
            </div>
            <div style={styles.fieldGroup}>
              <label style={styles.label}>確認密碼 <span style={styles.required}>*</span></label>
              <input
                type="password"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                style={styles.input}
                placeholder="再次輸入密碼"
                autoComplete="new-password"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              style={{ ...styles.submitBtn, opacity: loading ? 0.7 : 1 }}
            >
              {loading ? '註冊中...' : '建立帳號'}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}

const styles = {
  container: {
    minHeight: '100vh',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    padding: '20px',
  },
  card: {
    backgroundColor: '#fff',
    borderRadius: '16px',
    boxShadow: '0 20px 60px rgba(0, 0, 0, 0.3)',
    padding: '40px',
    width: '100%',
    maxWidth: '420px',
  },
  header: {
    textAlign: 'center',
    marginBottom: '24px',
  },
  title: {
    fontSize: '24px',
    fontWeight: 'bold',
    color: '#333',
    margin: '0 0 8px',
  },
  subtitle: {
    color: '#888',
    margin: 0,
    fontSize: '14px',
  },
  tabs: {
    display: 'flex',
    borderBottom: '2px solid #eee',
    marginBottom: '24px',
  },
  tab: {
    flex: 1,
    padding: '10px',
    border: 'none',
    background: 'none',
    cursor: 'pointer',
    fontSize: '16px',
    color: '#888',
    borderBottom: '2px solid transparent',
    marginBottom: '-2px',
    transition: 'all 0.2s',
  },
  activeTab: {
    color: '#764ba2',
    borderBottom: '2px solid #764ba2',
    fontWeight: 'bold',
  },
  errorBox: {
    backgroundColor: '#fff0f0',
    border: '1px solid #ffccc7',
    borderRadius: '8px',
    padding: '12px',
    marginBottom: '16px',
    color: '#cf1322',
    fontSize: '14px',
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: '16px',
  },
  fieldGroup: {
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
  },
  label: {
    fontSize: '14px',
    fontWeight: '500',
    color: '#555',
  },
  required: {
    color: '#ff4d4f',
  },
  input: {
    padding: '10px 14px',
    border: '1px solid #d9d9d9',
    borderRadius: '8px',
    fontSize: '15px',
    outline: 'none',
    transition: 'border-color 0.2s',
  },
  submitBtn: {
    padding: '12px',
    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    color: '#fff',
    border: 'none',
    borderRadius: '8px',
    fontSize: '16px',
    fontWeight: 'bold',
    cursor: 'pointer',
    marginTop: '8px',
  },
};

export default LoginPage;
