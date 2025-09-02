import { showError } from '../../Messages';
import cryptoJs from 'crypto-js';
// ❌ não precisa mais do btoa: a rsaEncrypt (Web Crypto) já retorna base64
// import btoa from 'btoa';
import { rsaEncrypt } from '../../../utils/Functions'; // deve retornar base64 (string)
import sessionManager from '../../../SessionManager';
import api from '../../../utils/Api';

export default function LoginController() {

   const efetuarLogin = (nomeDeUsuario, senha, recaptcha) => {
      return new Promise(async (resolve, reject) => {
         try {
            if (!nomeDeUsuario) {
               showError('Informe o nome de usuário');
               return reject();
            }
            if (!senha) {
               showError('Informe a senha');
               return reject();
            }

            // if (!recaptcha) {
            //    showError('Informe o captcha');
            //    return reject();
            // }

            const inputBase = {
               nomeDeUsuario,
               senha,
               tipoDeAcesso: 'ADM',
               origem: 'WEB',
            };

            // 1) Buscar chave pública (PEM)
            const publicKeyResp = await api.get('/publickey');
            // dependendo do backend, pode vir { key: '-----BEGIN PUBLIC KEY-----...' }
            const publicKeyPem =
               typeof publicKeyResp === 'string' ? publicKeyResp : publicKeyResp?.key || publicKeyResp?.pem || '';

            if (!publicKeyPem) {
               showError('Chave pública não recebida do servidor.');
               return reject();
            }

            // chave/iv simétricos aleatórios (base64) para o backend devolver o token
            const key = cryptoJs.enc.Base64.stringify(cryptoJs.lib.WordArray.random(32));
            const iv = cryptoJs.enc.Base64.stringify(cryptoJs.lib.WordArray.random(16));

            const symmetricKey = { key, iv };
            const input = { ...inputBase, symmetricKey };

            console.log('input', input);

            const inputString = JSON.stringify(input);

            // 3) Divide o payload em blocos (mantendo sua lógica)
            const BLOCK_LEN = 16; // atenção: RSA tem limite por tamanho da chave; isso é seu legado
            const blocks = inputString.match(new RegExp(`.{1,${BLOCK_LEN}}`, 'g')) || [];

            // 4) Criptografa todos os blocos (AGORA ASSÍNCRONO)
            // Se quiser paralelo: await Promise.all(blocks.map(b => rsaEncrypt(b, publicKeyPem)));
            const encryptedInput = [];
            for (let i = 0; i < blocks.length; i++) {
               const base64Chunk = await rsaEncrypt(blocks[i], publicKeyPem); // base64 já pronto
               encryptedInput.push(base64Chunk); // ❗ nada de btoa aqui
            }

            const tokenInput = {
               data: encryptedInput,
               recaptcha: recaptcha,
            };

            // 5) Faz login com o token cifrado

            const result = await api.post('/login/token', tokenInput, true, false);

            // 6) Descriptografa o token (compatível com seu backend)
            const decryptedToken = cryptoJs.AES.decrypt(result.token, cryptoJs.enc.Base64.parse(key), {
               iv: cryptoJs.enc.Base64.parse(iv),
            }).toString(cryptoJs.enc.Utf8);

            result.token = decryptedToken;
            sessionManager.setLogin(result);
            window.location = './adm';
            resolve();

         } catch (e) {
            const msg = e?.toString?.() || String(e);
            // showError(msg);
            reject(msg);
         }
      });
   };

   return {
      efetuarLogin,
   };
}
