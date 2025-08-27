import * as moment from 'moment';
// import crypto from 'crypto';
import { showError } from '../components/Messages';

export const stringToDate = (stringDate) => {
   if (!stringDate) return null;
   let result = moment(
      stringDate.substr(6, 4) + '-' + stringDate.substr(3, 2) + '-' + stringDate.substr(0, 2)
   ).toDate();
   return result;
};

export function addDays(date, days) {
   var result = new Date(date);
   result.setDate(result.getDate() + days);
   return result;
}

export const mesPorExtenso = (data ) => {
   var result = [
       'Janeiro',
     'Fevereiro',
     'Março',
      'Abril',
      'Maio',
      'Junho',
        'Julho',
      'Agosto',
       'Setembro',
        'Outubro',
       'Novembro',
      'Dezembro',
   ][data.getMonth()];
   return result;
};

export const isNumeric = (num) => {
   return !isNaN(num);
};

export const isString = (value) => {
   return typeof value === 'string' || value instanceof String;
};

export const dateToString = (stringDate,) => {
   if (!stringDate) return null;
   let date = new Date(stringDate);
   let result = moment(date).format('DD/MM/YYYY');
   return result;
};

export const dateTimeToString = (stringDate) => {
   if (!stringDate) return null;
   let date = new Date(stringDate);
   let result = moment(date).format( 'DD/MM/YYYY HH:mm:ss');
   return result;
};

export const timeToString = (stringDate) => {
   if (!stringDate) return null;
   let dateTime = dateTimeToString(stringDate);
   let result = dateTime.split(' ')[1];
   return result;
};

export const formatDate = (date, dateFormat = 'YYYY-MM-DD') => {
   if (!date) return null;
   let result = moment(date).format(dateFormat);
   return result;
};

export const formatDateTime = (date, dateFormat = 'YYYY-MM-DD HH:mm:ss') => {
   if (!date) return null;
   let result = moment(date).format(dateFormat);
   return result;
};

export const pad2 = (n) => {
   return (n < 10 ? '0' : '') + n;
};

export const pad = (number, length, char = '0') => {
   return number.toString().padStart(length, '0');
};

export const numberToCurrencyString = (number) => {
   if (number === null || number === undefined) return null;
   if (isNaN(number)) return null;
   let result = new Intl.NumberFormat('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
      signDisplay: 'auto',
   }).format(number);
   return result;
};

export const create_UUID = () => {
   var dt = new Date().getTime();
   var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      var r = (dt + Math.random() * 16) % 16 | 0;
      dt = Math.floor(dt / 16);
      // eslint-disable-next-line
      return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
   });
   return uuid;
};

export const numberToString = (number, minimumFractionDigits = 0, maximumFractionDigits = 6) => {
   if (number === null || number === undefined) return null;
   if (isNaN(number)) return null;
   let result = new Intl.NumberFormat('pt-BR', {
      minimumFractionDigits: minimumFractionDigits,
      maximumFractionDigits: maximumFractionDigits,
      signDisplay: 'auto',
   }).format(number);
   return result;
};

export const numberToString2 = (number, minimumFractionDigits = 0) => {
   if (number === null || number === undefined) return null;
   if (isNaN(number)) return null;
   let result = new Intl.NumberFormat( 'pt-BR', {
      minimumFractionDigits: minimumFractionDigits,
      signDisplay: 'auto',
   }).format(number);
   return result;
};

export const replaceAll = (string, search, replace) => {
   return string.split(search).join(replace);
};

export const removeMask = (text) => {
   if (!text) return text;
   let result = replaceAll(text, '.', '');
   result = replaceAll(result, '-', '');
   return result;
};

export const getEnderecoCompleto = (endereco) => {
   if (!endereco) return null;

   if (!endereco.pais || (endereco.pais && endereco.pais.codigo === '+55')) {
      return replaceAll(
         [
            endereco.logradouro,
            endereco.numero,
            endereco.complemento,
            endereco.bairro,
            endereco.cep ? 'CEP ' + endereco.cep : '',
            endereco.cidade.nome + ' / ' + endereco.cidade.estado.uf,
         ].join(', '),
         ', , ',
         ', '
      );
   } else {
      return replaceAll([endereco.linha1, endereco.linha2, endereco.linha3].join('\n'), ', , ', ', ').trim();
   }
};

export const horaMaiorOuIgual = (a, b) => {
   let result = parseInt(a.replace(':', '')) >= parseInt(b.replace(':', ''));
   return result;
};

export const horaMenorOuIgual = (a, b) => {
   let result = parseInt(a.replace(':', '')) <= parseInt(b.replace(':', ''));
   return result;
};

export const calcularDiferencaEntreHorasEmMinutos = (horaInicial, horaFinal) => {
   let data = moment();
   let horaInicial1 = moment(data.format('YYYY-MM-DD ' + horaInicial));
   let horaFinal1 = moment(data.format('YYYY-MM-DD ' + horaFinal));
   let result = horaFinal1.diff(horaInicial1, 'minutes');
   return result;
};

export const converterHorasParaMinutos = (hora) => {
   let result = calcularDiferencaEntreHorasEmMinutos('00:00:00', hora);
   return result;
};

export const converterMinutosParaHora = (minutos) => {
   let data = moment();
   let dataBase = moment(data.format('YYYY-MM-DD'));
   dataBase.add(minutos, 'minutes');
   let result = dataBase.format('HH:mm');
   return result;
};

export const getNomeDoDiaDaSemana = (diaDaSemana) => {
   let nomes = [
     'Domingo',
     'Segunda-feira',
      'Terça-feira',
       'Quarta-feira',
    'Quinta-feira',
     'Sexta-feira',
      'Sábado',
   ];
   let result = nomes[diaDaSemana - 1];
   return result;
};

export const getTelefones = (item) => {
   let result = '';
   item.telefones.forEach((telefone) => {
      result += telefone.numeroComDDD + ', ';
   });
   if (result[result.length - 2] === ',') {
      result = result.substr(0, result.length - 2);
   }
   return result;
};

export const generatePassword = (numberOnly, length) => {
   var charset = numberOnly ? 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789' : '0123456789',
      retVal = '';
   for (var i = 0, n = charset.length; i < length; ++i) {
      retVal += charset.charAt(Math.floor(Math.random() * n));
   }
   return retVal;
};

export const retirarAcentos = (str) => {
   if (!str) {
      return '';
   }
   let com_acento = 'ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝŔÞßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿŕ';

   let sem_acento = 'AAAAAAACEEEEIIIIDNOOOOOOUUUUYRsBaaaaaaaceeeeiiiionoooooouuuuybyr';
   let novastr = '';
   for (let i = 0; i < str.length; i++) {
      let troca = false;
      for (let a = 0; a < com_acento.length; a++) {
         if (str.substr(i, 1) === com_acento.substr(a, 1)) {
            novastr += sem_acento.substr(a, 1);
            troca = true;
            break;
         }
      }
      if (troca === false) {
         novastr += str.substr(i, 1);
      }
   }
   return novastr;
};

export const inputToUpper = (e) => {
   var start = e.target.selectionStart;
   // var end = e.target.selectionEnd;
   e.target.value = ('' + e.target.value).toUpperCase();
   e.target.setSelectionRange(start, start);
   e.preventDefault();
};
 
export const buildQueryString = (pageSize, skip, orderBy, filter) => {
   let query = '?';
   if (pageSize) query += 'pagesize=' + pageSize.toString() + '&';
   if (skip) query += 'skip=' + skip.toString() + '&';
   if (orderBy) query += 'orderBy=' + orderBy.toString() + '&';
   if (filter) {
      let filterString = Object.keys(filter)
         .map((key) => {
            let result = null;
            let name = encodeURIComponent(key);
            let value = encodeURIComponent(filter[key]);
            if (value !== 'null' && value !== 'undefined') {
               result = `${name}=${value}`;
            }
            return result;
         })
         .filter((i) => i)
         .join('&');
      query += filterString ? 'query=' + encodeURIComponent(filterString) + '&' : '';
   }
   return query.substr(0, query.length - 1);
};

export const objectToQueryString = (filter) => {
   let query = '';

   if (filter) {
      query = Object.keys(filter)
         .map((key) => {
            let result = null;
            let name = encodeURIComponent(key);
            let value = encodeURIComponent(filter[key]);
            if (value !== 'null' && value !== 'undefined') {
               result = `${name}=${value}`;
            }
            return result;
         })
         .filter((i) => i)
         .join('&');
   }
   return query;
};

// converte PEM em ArrayBuffer para importar no Web Crypto
function pemToArrayBuffer(pem) {
   const b64 = pem
      .replace(/-----BEGIN PUBLIC KEY-----/, '')
      .replace(/-----END PUBLIC KEY-----/, '')
      .replace(/\s+/g, '');
   const binary = window.atob(b64);
   const buf = new Uint8Array(binary.length);
   for (let i = 0; i < binary.length; i++) {
      buf[i] = binary.charCodeAt(i);
   }
   return buf.buffer;
}

export async function rsaEncrypt(data, pemKey) {
    // 1. Importa a chave pública PEM
   const key = await crypto.subtle.importKey(
      'spki',
      pemToArrayBuffer(pemKey),
      {
         name: 'RSA-OAEP',
         hash: 'SHA-1', // equivalente ao oaepHash: 'sha1'
      },
      false,
      ['encrypt']
   );

   // 2. Converte string para ArrayBuffer
   const enc = new TextEncoder();
   const encoded = enc.encode(data);

   // 3. Criptografa
   const encrypted = await crypto.subtle.encrypt(
      {
         name: 'RSA-OAEP',
      },
      key,
      encoded
   );

   // 4. Retorna em base64 (pode trocar para hex, se preferir)
   return window.btoa(String.fromCharCode(...new Uint8Array(encrypted)));
}

// Helpers (coloque num util compartilhado, se preferir)
export function base64ToUint8(base64) {
  const bin = atob(base64);
  const out = new Uint8Array(bin.length);
  for (let i = 0; i < bin.length; i++) out[i] = bin.charCodeAt(i);
  return out;
}

export  function uint8ToBase64(u8) {
  let bin = '';
  for (let i = 0; i < u8.length; i++) bin += String.fromCharCode(u8[i]);
  return btoa(bin);
}

// helper: ArrayBuffer -> base64
export const arrayBufferToBase64 = (buffer) => {
   let binary = '';
   const bytes = new Uint8Array(buffer);
   const len = bytes.byteLength;
   for (let i = 0; i < len; i++) binary += String.fromCharCode(bytes[i]);
   return window.btoa(binary);
};

export function concatUint8Arrays(arrays) {
  
  const totalLength = arrays.reduce((acc, value) => acc + value.length, 0);

  // cria buffer final
  const result = new Uint8Array(totalLength);

  // copia pedaços
  let offset = 0;
  for (const arr of arrays) {
    result.set(arr, offset);
    offset += arr.length;
  }
  return result;
}

export function concatArrayBuffers(buffers) {
  const totalLength = buffers.reduce((acc, b) => acc + b.byteLength, 0);
  const tmp = new Uint8Array(totalLength);

  let offset = 0;
  for (const b of buffers) {
    tmp.set(new Uint8Array(b), offset);
    offset += b.byteLength;
  }
  console.log(tmp.buffer);
  return tmp.buffer; // retorna ArrayBuffer
}

export const validarEndereco = (paisObrigatorio, endereco, reject) => {
   if (paisObrigatorio && !endereco.pais) {
      showError('Informe o país.');
      reject();
      return false;
   }

   if ((paisObrigatorio && endereco.pais.codigo === '+55') || !paisObrigatorio) {
      if (!endereco.cep) {
         showError('Informe o CEP.');
         reject();
         return false;
      }

      if (!endereco.logradouro) {
         showError('Informe o logradouro.');
         reject();
         return false;
      }

      if (!endereco.numero) {
         showError('Informe o número do endereço.');
         reject();
         return false;
      }

      if (!endereco.bairro) {
         showError('Informe o bairro.');
         reject();
         return false;
      }

      if (!endereco.cidade) {
         showError('Informe a cidade.');
         reject();
         return false;
      }
   }
   return true.valueOf;
};

export const validarTelefone = (paisPadrao, telefone, reject) => {
   if (!telefone.pais) {
      telefone.pais = paisPadrao;
   }

   if (!telefone.pais) {
      showError('Informe o país.');
      reject();
      return false;
   }

   if (!telefone.ddd) {
      showError('Informe o DDD.');
      reject();
      return false;
   }

   if (isNaN(telefone.ddd)) {
      showError('Informe apenas números para o DDD.');
      reject();
      return false;
   }

   if (!telefone.numero) {
      showError('Informe o número.');
      reject();
      return false;
   }

   if (isNaN(telefone.numero)) {
      showError('Informe apenas números.');
      reject();
      return false;
   }

   return true;
};

export const generateUUID = () => {
   // Public Domain/MIT
   var d = new Date().getTime(); //Timestamp
   var d2 = (typeof performance !== 'undefined' && performance.now && performance.now() * 1000) || 0; //Time in microseconds since page-load or 0 if unsupported
   return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16; //random number between 0 and 16
      if (d > 0) {
         //Use timestamp until depleted
         r = (d + r) % 16 | 0;
         d = Math.floor(d / 16);
      } else {
         //Use microseconds since page-load if supported
         r = (d2 + r) % 16 | 0;
         d2 = Math.floor(d2 / 16);
      }
      return (c === 'x' ? r : (r & 0x3) | 0x8).toString(16);
   });
};

export const toEntityReference = (i) => {
   let result = null;
   if (i && i.id !== null && i.id !== undefined) {
      result = { id: i.id };
   }
   return result;
};

export const getBase64ImageSize = (base64) => {
   return new Promise((resolve, reject) => {
      let img = document.createElement('img');
      img.onload = function () {
         resolve({
            height: img.height,
            width: img.width,
         });
      };
      img.onerror = function (e) {
         console.error(e);
      };
      img.src = base64;
   });
};

export const emptyStrIfNull = (s, emptyStr = '') => {
   return s ? s : emptyStr;
};

export const downloadBase64File = (contentBase64, fileName, dataType) => {
   const linkSource = `data:${dataType};base64,${contentBase64}`;
   const downloadLink = document.createElement('a');
   document.body.appendChild(downloadLink);

   downloadLink.href = linkSource;
   downloadLink.target = '_self';
   downloadLink.download = fileName;
   downloadLink.click();
};

export const getTempoDecorrido = (data) => {
   const agora = new Date();
   const diffMs = agora - new Date(data);

   const segundos = Math.floor(diffMs / 1000);
   const minutos = Math.floor(segundos / 60);
   const horas = Math.floor(minutos / 60);
   const dias = Math.floor(horas / 24);

   if (segundos < 60) {
      return `${segundos} segundo${segundos !== 1 ? 's' : ''}`;
   } else if (minutos < 60) {
      return `${minutos} minuto${minutos !== 1 ? 's' : ''}`;
   } else if (horas < 24) {
      const restoMinutos = minutos % 60;
      return restoMinutos > 0
         ? `${horas} hora${horas !== 1 ? 's' : ''} e ${restoMinutos} minuto${restoMinutos !== 1 ? 's' : ''}`
         : `${horas} hora${horas !== 1 ? 's' : ''}`;
   } else {
      return `${dias} dia${dias !== 1 ? 's' : ''}`;
   }
};
