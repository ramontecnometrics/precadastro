import React, { useState, useEffect, useCallback } from 'react';
import IconButton from './IconButton';
import { Form } from 'react-bootstrap';
import styled from 'styled-components';
import { faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { faTrashAlt } from '@fortawesome/free-regular-svg-icons';
import { showConfirm } from './Messages';
import { LayoutParams } from '../config/LayoutParams';
import { arrayBufferToBase64 } from '../utils/Functions';
import api from '../utils/Api';

export default function Avatar(props) {
   const [base64, setBase64] = useState(null);

   // ========= carregar imagem =========
   const load = useCallback(() => {
      if (props.image && props.image.nome) {
         console.log('Avatar.load()');
         api.get('/file?filename=' + props.image.nome, true, 'arraybuffer')
            .then((data) => {
               const b64 = arrayBufferToBase64(data);
               setBase64('data:image/png;base64, ' + b64);
            })
            .catch(() => {});
      }
   }, [props.image]);

   useEffect(() => {
      load();
   }, [load]);

   // ========= mudança de arquivo =========
   const onChange = (event) => {
      const tg = event.target;
      if (event.target.files.length > 0) {
         setBase64(null);
         let file = event.target.files[0];
         const fileReader = new FileReader();
         fileReader.addEventListener(
            'load',
            function () {
               const input = {
                  tipo: props.mimeType ? props.mimeType : file.type,
                  base64: fileReader.result,
                  descricao: file.name,
                  temporario: props.temporary,
               };
               api.post('/file', input).then((result) => {
                  if (props.onChange) {
                     props.onChange({
                        id: result.id,
                        nome: result.nome,
                        base64: fileReader.result,
                     });
                  }
                  tg.value = null;
                  load();
               });
            },
            false
         );
         fileReader.readAsDataURL(file);
      }
   };

   // ========= excluir =========
   const excluir = (event) => {
      const tg = event.target;
      showConfirm("Deseja realmente excluir a foto?", () => {
         if (props.onChange) {
            props.onChange(null);
         }
         if (tg) tg.value = null;
         setBase64(null);
      });
   };

   return (
      <div style={{ textAlign: 'right', maxWidth: 150, marginLeft: -20, position: 'relative' }}>
         <Form.Label
            style={{ cursor: props.readOnly ? 'default' : 'pointer', marginBottom: 13 }}
            title={props.image ? "Alterar foto" : "Carregar foto"}
         >
            <input
               type='file'
               accept={'image/png, image/jpeg'}
               onChange={onChange}
               style={{ position: 'absolute', top: -1000 }}
               tabIndex={-1}
               disabled={props.readOnly}
            />
            {props.image && props.image.nome ? (
               <ImgRounded alt='' url={base64} width={props.width} />
            ) : (
               <ImgNone alt='' width={props.width}>
                  <IconButton style={{ color: LayoutParams.colors.corSecundaria }} icon={faUserCircle} />
               </ImgNone>
            )}
         </Form.Label>
         {props.image && props.image.nome && !props.readOnly && (
            <div
               className='hide-when-readonly'
               style={{
                  color: 'initial',
                  position: 'absolute',
                  right: 3,
                  top: 85,
                  fontSize: 20,
                  cursor: 'pointer',
               }}
               title="Excluir foto"
               onClick={excluir}
            >
               <IconButton style={{ color: LayoutParams.colors.corSecundaria }} icon={faTrashAlt} />
            </div>
         )}
      </div>
   );
}

// ========= styled =========

const ImgRounded = styled.div`
   border-radius: 50%;
   height: ${(props) => (props.width ? props.width : 100)}px;
   width: ${(props) => (props.width ? props.width : 100)}px;
   background-position-x: center;
   background-size: cover;
   background-image: url("${(props) => props.url}");
`;

const ImgNone = styled.div`
   object-fit: cover;
   border-radius: 50%;
   height: ${(props) => (props.width ? props.width : 100)}px;
   width: ${(props) => (props.width ? props.width : 100)}px;
   font-size: 110px;
   line-height: 0px;

   i {
      text-decoration: none;
      color: #888;
   }
`;
