const template = document.createElement('template');
template.innerHTML = `
<style>
  :host { display: block; padding: 20px; }
  .asyntai-wrap { max-width: 960px; }
  .asyntai-status { font-size: 14px; margin-bottom: 16px; }
  .status-connected { color: #008a20; font-weight: 600; }
  .status-disconnected { color: #a00; font-weight: 600; }
  .asyntai-card { max-width: 820px; margin: 20px 0; padding: 32px; border: 1px solid #e9e9eb; border-radius: 8px; background: #fff; text-align: center; }
  .asyntai-card-title { font-size: 20px; font-weight: 700; margin-bottom: 8px; color: #1b264f; }
  .asyntai-card-text { font-size: 16px; margin-bottom: 16px; color: #666; }
  .asyntai-tip { margin-top: 16px; font-size: 14px; color: #666; }
  .asyntai-tip a { color: #3182ce; text-decoration: underline; }
  .asyntai-fallback { margin-top: 12px; color: #666; font-size: 14px; }
  .asyntai-fallback a { color: #3182ce; text-decoration: underline; }
  .btn { display: inline-block; padding: 10px 20px; border: none; border-radius: 4px; font-size: 14px; cursor: pointer; text-decoration: none; }
  .btn-primary { background: #3544b1; color: #fff; }
  .btn-primary:hover { background: #2a3a9e; }
  .btn-secondary { background: #f3f3f5; color: #303033; margin-left: 8px; padding: 6px 12px; }
  .btn-secondary:hover { background: #e9e9eb; }
  .alert { padding: 12px 16px; margin-bottom: 16px; border-radius: 4px; font-size: 14px; display: none; }
  .alert-success { background-color: #d4edda; border-left: 4px solid #00a32a; color: #155724; display: block; }
  .alert-error { background-color: #f8d7da; border-left: 4px solid #d63638; color: #721c24; display: block; }
  h1 { font-size: 24px; font-weight: 700; margin-bottom: 20px; color: #1b264f; }
</style>
<div class="asyntai-wrap">
  <h1>Asyntai AI Chatbot</h1>
  <p class="asyntai-status">
    Status: <span id="status-text" class="status-disconnected">Not connected</span>
    <span id="account-email"></span>
    <button id="reset-btn" class="btn btn-secondary" style="display:none;">Reset</button>
  </p>
  <div id="alert" class="alert"></div>
  <div id="connected-box" style="display:none;">
    <div class="asyntai-card">
      <div class="asyntai-card-title">Asyntai is now enabled</div>
      <div class="asyntai-card-text">Set up your AI chatbot, review chat logs and more:</div>
      <a class="btn btn-primary" href="https://asyntai.com/dashboard" target="_blank" rel="noopener">Open Asyntai Panel</a>
      <div class="asyntai-tip">
        <strong>Tip:</strong> If you want to change how the AI answers, please <a href="https://asyntai.com/dashboard#setup" target="_blank" rel="noopener">go here</a>.
      </div>
    </div>
  </div>
  <div id="connect-box">
    <div class="asyntai-card">
      <div class="asyntai-card-text">Create a free Asyntai account or sign in to enable the chatbot</div>
      <button id="connect-btn" class="btn btn-primary">Get started</button>
      <div class="asyntai-fallback">If it doesn't work, <a id="fallback-link" href="https://asyntai.com/wp-auth?platform=umbraco" target="_blank" rel="noopener">open the connect window</a>.</div>
    </div>
  </div>
</div>
`;

class AsyntaiDashboard extends HTMLElement {
  constructor() {
    super();
    this.attachShadow({ mode: 'open' });
    this.shadowRoot.appendChild(template.content.cloneNode(true));

    this.connected = false;
    this.siteId = '';
    this.accountEmail = '';
    this.expectedOrigin = 'https://asyntai.com';
    this.connectUrl = this.expectedOrigin + '/wp-auth?platform=umbraco';
  }

  connectedCallback() {
    this.shadowRoot.getElementById('connect-btn').addEventListener('click', () => this.openPopup());
    this.shadowRoot.getElementById('reset-btn').addEventListener('click', () => this.reset());
    this.loadSettings();
  }

  async loadSettings() {
    try {
      const response = await fetch('/umbraco/api/asyntai/settings', { credentials: 'same-origin' });
      const data = await response.json();
      if (data) {
        this.connected = data.connected || false;
        this.siteId = data.siteId || '';
        this.accountEmail = data.accountEmail || '';
        this.updateUI();
      }
    } catch (error) {
      this.showAlert('Failed to load settings: ' + error.message, false);
    }
  }

  updateUI() {
    const statusText = this.shadowRoot.getElementById('status-text');
    const accountEmail = this.shadowRoot.getElementById('account-email');
    const resetBtn = this.shadowRoot.getElementById('reset-btn');
    const connectedBox = this.shadowRoot.getElementById('connected-box');
    const connectBox = this.shadowRoot.getElementById('connect-box');

    if (this.connected) {
      statusText.textContent = 'Connected';
      statusText.className = 'status-connected';
      accountEmail.textContent = this.accountEmail ? ' as ' + this.accountEmail : '';
      resetBtn.style.display = 'inline-block';
      connectedBox.style.display = 'block';
      connectBox.style.display = 'none';
    } else {
      statusText.textContent = 'Not connected';
      statusText.className = 'status-disconnected';
      accountEmail.textContent = '';
      resetBtn.style.display = 'none';
      connectedBox.style.display = 'none';
      connectBox.style.display = 'block';
    }
  }

  showAlert(message, success) {
    const alert = this.shadowRoot.getElementById('alert');
    alert.textContent = message;
    alert.className = 'alert ' + (success ? 'alert-success' : 'alert-error');
  }

  openPopup() {
    const state = 'umbraco_' + Math.random().toString(36).substr(2, 9);
    const url = this.connectUrl + '&state=' + encodeURIComponent(state);
    const w = 800, h = 720;
    const y = window.top.outerHeight / 2 + window.top.screenY - (h / 2);
    const x = window.top.outerWidth / 2 + window.top.screenX - (w / 2);
    const popup = window.open(url, 'asyntai_connect', 'toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=' + w + ',height=' + h + ',top=' + y + ',left=' + x);
    if (!popup) {
      this.showAlert('Popup blocked. Please allow popups or use the link below.', false);
      return;
    }
    this.pollForConnection(state);
  }

  pollForConnection(state) {
    let attempts = 0;
    const self = this;
    function check() {
      if (attempts++ > 60) return;
      const script = document.createElement('script');
      const cb = 'asyntai_cb_' + Date.now();
      script.src = self.expectedOrigin + '/connect-status.js?state=' + encodeURIComponent(state) + '&cb=' + cb;
      window[cb] = function(data) {
        try { delete window[cb]; } catch (e) {}
        if (data && data.site_id) { self.saveConnection(data); return; }
        setTimeout(check, 500);
      };
      script.onerror = function() { setTimeout(check, 1000); };
      document.head.appendChild(script);
    }
    setTimeout(check, 800);
  }

  async saveConnection(data) {
    this.showAlert('Asyntai connected. Saving...', true);
    const payload = {
      siteId: data.site_id || '',
      scriptUrl: data.script_url || null,
      accountEmail: data.account_email || null
    };
    try {
      const response = await fetch('/umbraco/api/asyntai/save', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'same-origin',
        body: JSON.stringify(payload)
      });
      const json = await response.json();
      if (!json || !json.success) throw new Error(json?.error || 'Save failed');
      this.showAlert('Asyntai connected. Chatbot enabled on all pages.', true);
      this.connected = true;
      this.siteId = payload.siteId;
      this.accountEmail = payload.accountEmail || '';
      this.updateUI();
    } catch (error) {
      this.showAlert('Could not save settings: ' + error.message, false);
    }
  }

  async reset() {
    if (!confirm('Are you sure you want to reset the Asyntai connection?')) return;
    try {
      const response = await fetch('/umbraco/api/asyntai/reset', {
        method: 'POST',
        credentials: 'same-origin'
      });
      const json = await response.json();
      if (!json || !json.success) throw new Error(json?.error || 'Reset failed');
      this.showAlert('Connection reset successfully.', true);
      this.connected = false;
      this.siteId = '';
      this.accountEmail = '';
      this.updateUI();
    } catch (error) {
      this.showAlert('Reset failed: ' + error.message, false);
    }
  }
}

customElements.define('asyntai-dashboard', AsyntaiDashboard);

export default AsyntaiDashboard;
