import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

import Spin from "antd/es/spin";
import useApp from './hooks/useApp';

import App from './App.jsx'
import { AppProviders } from './providers/App'

import './assets/style/main.css';

const SiteLoad = () => {

  const { isLoading } = useApp();

  return <Spin spinning={isLoading} fullscreen />;
}

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <AppProviders>
      <App />
      <SiteLoad />
    </AppProviders>
  </StrictMode>,
)
