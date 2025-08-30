import axios from 'axios';
import sessionManager from './../SessionManager';
import { ConnectionParams } from './../config/ConnectionParams';
import { rsaEncrypt } from './Functions';
import { showError } from '../components/Messages';

const urlBase = ConnectionParams.apiUrl;
export const DEBUG = false;

class Api {
   constructor() {
      this.cache = [];
      this.loading = false;

      this.insertToCache = this.insertToCache.bind(this);
      this.getFromCache = this.getFromCache.bind(this);
      this.getAll = this.getAll.bind(this);
      this.startLoading = this.startLoading.bind(this);
      this.finishLoading = this.finishLoading.bind(this);
   }

   insertToCache = (url, data) => {
      const existing = this.cache.findIndex((i) => i.url === url);
      if (existing === -1) {
         this.cache.push({ url, data });
      } else {
         this.cache[existing] = { url, data }; // atualiza
      }
   };

   getFromCache = (url) => {
      const item = this.cache.find((i) => i.url === url);
      return item || null;
   };

   urlBase = () => urlBase;

   getOptions = (queryParams, responseType) => {
      let login = sessionManager.getLogin();
      let headers = {};

      if (login?.token) headers.Authorization = login.token;
      if (login?.idDoUsuario) headers.UserId = login.idDoUsuario;      
      if (login?.impersonatedBy) headers.RepresentingId = login.impersonatedBy;

      return {
         responseType,
         headers,
         params: queryParams,
      };
   };

   startLoading = () => {
      this.loading = true;
      setTimeout(() => {
         if (this.loading) {
            const el = document.getElementById('loading');
            if (el) el.className = 'loadingDiv';
         }
      }, 5000);
   };

   finishLoading = () => {
      this.loading = false;
      const el = document.getElementById('loading');
      if (el) el.className = 'loadingDiv hide';
   };

   // ========= HTTP =========
   post = (url, data, useProgress = true, showErrors = true) =>
      new Promise((resolve, reject) => {
         if (useProgress) this.startLoading();
         axios
            .post(urlBase + url, data, this.getOptions())
            .then((result) => {
               this.finishLoading();
               resolve(result.data);
            })
            .catch((e) => {
               this.finishLoading();
               showErrors ? this.handleErrorMessage(e, reject) : reject(this.getErrorMessage(e));
            });
      });

   put = (url, data, useProgress = true, showErrors = true) =>
      new Promise((resolve, reject) => {
         if (useProgress) this.startLoading();
         axios
            .put(urlBase + url, data, this.getOptions())
            .then((result) => {
               this.finishLoading();
               resolve(result.data);
            })
            .catch((e) => {
               this.finishLoading();
               showErrors ? this.handleErrorMessage(e, reject) : reject(this.getErrorMessage(e));
            });
      });

   delete = (url) =>
      new Promise((resolve, reject) => {
         this.startLoading();
         axios
            .delete(urlBase + url, this.getOptions())
            .then((result) => {
               this.finishLoading();
               resolve(result.data);
            })
            .catch((e) => {
               this.finishLoading();
               this.handleErrorMessage(e, reject);
            });
      });

   get = (url, useCache = false, responseType = '', useProgress = true, showErrors = true) =>
      new Promise((resolve, reject) => {
         let cachedResult = useCache ? this.getFromCache(url) : null;

         if (!cachedResult) {
            if (useProgress) this.startLoading();
            axios
               .get(urlBase + url, this.getOptions(null, responseType))
               .then((result) => {
                  this.finishLoading();
                  if (useCache) this.insertToCache(url, result.data);

                  // corrigido length
                  resolve(
                     Array.isArray(result.data) && result.data.length > 0 ? result.data[0] : result.data
                  );
               })
               .catch((e) => {
                  this.finishLoading();
                  showErrors ? this.handleErrorMessage(e, reject) : reject(e);
               });
         } else {
            resolve(cachedResult.data);
         }
      });

   getAll = (url, useCache = false) =>
      new Promise((resolve, reject) => {
         let cachedResult = useCache ? this.getFromCache(url) : null;

         if (!cachedResult) {
            this.startLoading();
            axios
               .get(urlBase + url, this.getOptions())
               .then((result) => {
                  this.finishLoading();
                  if (useCache) this.insertToCache(url, result.data);
                  resolve(result.data);
               })
               .catch((e) => {
                  this.finishLoading();
                  this.handleErrorMessage(e, reject);
               });
         } else {
            resolve(cachedResult.data);
         }
      });

   query = (url, queryParams) =>
      new Promise((resolve, reject) => {
         this.startLoading();
         axios
            .get(urlBase + url, this.getOptions(queryParams))
            .then((result) => {
               this.finishLoading();
               resolve(result.data);
            })
            .catch((e) => {
               this.finishLoading();
               this.handleErrorMessage(e, reject);
            });
      });

   // ========= Erros =========
   handleErrorMessage = (error, reject) => {
      let message = this.getErrorMessage(error);
      let details = this.getErrorDetails(error);
      let stack = this.getErrorStack(error);
      showError(message, details, stack);
      if (reject) reject(message);
   };

   getErrorMessage = (error) => {
      let mensagem = '';
      if (error.response?.data?.errorMessage) {
         mensagem = error.response.data.errorMessage;
      } else if (error.response?.data?.ExceptionMessage) {
         mensagem = error.response.request.response;
      } else if (error.response?.request?.response) {
         mensagem = error.response.request.response;
      } else if (error.response?.statusText) {
         mensagem = error.response.statusText;
      } else {
         mensagem = error.message;
      }

      if (mensagem === 'Network Error') {
         mensagem = "Servidor indisponível.";
      }
      return mensagem;
   };

   getErrorDetails = (error) => error.response?.data?.details || '';
   getErrorStack = (error) => error.response?.data?.stackTrace || '';

   // ========= Protected =========
   protectedPost = (url, data) => this._protectedRequest('post', url, data);
   protectedPut = (url, data) => this._protectedRequest('put', url, data);

   _protectedRequest = async (method, url, data) => {
      try {
         const publicKeyResp = await this.get('/publickey');
         const publicKey =
            typeof publicKeyResp === 'string'
               ? publicKeyResp
               : publicKeyResp?.key || publicKeyResp?.pem || '';

         const inputString = JSON.stringify(data);
         const encoder = new TextEncoder();
         const inputBytes = encoder.encode(inputString);

         // dividir em blocos seguros
         const BLOCK_LEN = 100;
         let blocks = [];
         for (let i = 0; i < inputBytes.length; i += BLOCK_LEN) {
            blocks.push(inputBytes.slice(i, i + BLOCK_LEN));
         }

         const encryptedInput = await Promise.all(blocks.map((b) => rsaEncrypt(b, publicKey)));
         return this[method](url, { data: encryptedInput });
      } catch (e) {
         return Promise.reject(e);
      }
   };
}

const api = new Api();
export default api;