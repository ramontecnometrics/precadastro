import IconButton from './IconButton';
import React, { useState, useEffect, Fragment } from 'react';
import { Form } from 'react-bootstrap';
import styled from 'styled-components';
import { faImage } from '@fortawesome/free-solid-svg-icons';
import { faTrashAlt } from '@fortawesome/free-regular-svg-icons';
import { showConfirm } from './Messages';
import { arrayBufferToBase64 } from '../utils/Functions';
import api from '../utils/Api';

export default function Image(props) {
   const [state, setState] = useState({ base64: null, svgImage: '' });

   useEffect(() => {
      load();
   }, [props.image]);

   const load = () => {
      if (props.image && props.image.nome) {
         api.get('/file?filename=' + props.image.nome, true, 'arraybuffer').then((data) => {
            if (props.image.tipo === 'image/svg+xml') {
               const svgText = new TextDecoder('utf-8').decode(new Uint8Array(data));
               setState({ svgImage: svgText });
            } else {
               const base64 = arrayBufferToBase64(data);
               setState({ base64: 'data:image/png;base64,' + base64 });
            }
         });
      }
   };

   const onChange = (event) => {
      var tg = event.target;
      if (event.target.files.length > 0) {
         let file = event.target.files[0];
         var fileReader = new FileReader();
         fileReader.addEventListener(
            'load',
            function () {
               var input = {
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
                        tipo: result.tipo,
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

   const excluir = (event) => {
      var tg = event.target;
      showConfirm('Deseja realmente excluir a imagem?', () => {
         if (props.onChange) {
            props.onChange(null);
         }
         tg.value = null;
         setState({ base64: null });
      });
   };

   return (
      <Fragment>
         <Form.Label
            style={{
               cursor: props.cursor ?? 'pointer',
               margin: 0,
               textAlign: 'center',
               width: props.width,
            }}
            title={props.image && props.image ? 'Alterar imagem' : 'Carregar imagem'}
         >
            {!props.readOnly && (
               <Form.File
                  accept={'image/png, image/jpeg, image/svg+xml'}
                  onChange={onChange}
                  style={{ position: 'absolute', top: -1000 }}
                  tabIndex={-1}
               />
            )}

            {props.image && props.image.nome ? (
               <Fragment>
                  {state.base64 && (
                     <img
                        alt=''
                        src={state.base64}
                        height={props.height}
                        width={props.width}
                        style={{ maxHeight: '100%', maxWidth: '100%' }}
                        onLoad={() => {
                           if (props.onLoad) {
                              props.onLoad(state.base64);
                           }
                        }}
                     />
                  )}
                  {state.svgImage && (
                     <SvgStyled fill={props.fill ? props.fill : 'transparent'}>
                        <SvgRenderer svgString={state.svgImage} height={props.height} width={props.width} />
                     </SvgStyled>
                  )}
               </Fragment>
            ) : (
               <ImgNone alt='' width={props.width}>
                  <IconButton style={{ color: '#999', margin: 'auto', height: props.height }} icon={faImage} />
               </ImgNone>
            )}
         </Form.Label>
         {props.image && props.image.nome && !props.readOnly && (
            <div
               style={{
                  color: 'initial',
                  position: 'absolute',
                  right: 8,
                  bottom: -30,
                  fontSize: 20,
                  cursor: 'pointer',
                  width: props.width,
               }}
               title={'Excluir imagem'}
               onClick={excluir}
            >
               <IconButton icon={faTrashAlt} style={{ margin: 'auto', float: 'right' }} />
            </div>
         )}
      </Fragment>
   );
}

const ImgNone = styled.div`
   object-fit: cover;
   height: ${(props) => (props.height ? props.height : '100%')};
   max-height: ${(props) => (props.maxHeight ? props.maxHeight : '100%')};
   width: ${(props) => (props.width ? props.width : '100%')};
   font-size: 110px;
   line-height: 0px;

   i {
      text-decoration: none;
      color: #888;
   }
`;

function SvgRenderer({ svgString, height, width }) {
   return <div style={{ height: height, width: width }} dangerouslySetInnerHTML={{ __html: svgString }} />;
}

const SvgStyled = styled.div`
   .st0 {
      fill: ${(props) => props.fill} !important;
   }
`;
