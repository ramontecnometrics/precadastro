import React, { useState } from 'react';
import { Form, InputGroup } from 'react-bootstrap';
import IconButton from './IconButton';
import { faTrashAlt, faFolderOpen } from '@fortawesome/free-regular-svg-icons';
import { showConfirm } from './Messages';
import { faDownload } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { LayoutParams } from '../config/LayoutParams';
import api from '../utils/Api';

export default function Document(props) {
   const [state, setState] = useState({
      descricao: props.defaultValue ? props.defaultValue.descricao : null,
   });

   const downloadBase64File = (contentBase64, fileName, dataType) => {
      const linkSource = `data:${dataType};base64,${contentBase64}`;
      const downloadLink = document.createElement('a');
      document.body.appendChild(downloadLink);

      downloadLink.href = linkSource;
      downloadLink.target = '_self';
      downloadLink.download = fileName;
      downloadLink.click();
   };

   const baixar = () => {
      api
         .get('/file/base64/' + props.defaultValue.nome)
         .then((result) => {
            downloadBase64File(
               result,
               props.defaultValue.descricao ? props.defaultValue.descricao : props.defaultValue.nome,
               props.defaultValue.tipo
            );
         })
         .catch((e) => {});
   };

   const onChange = (event) => {
      var tg = event.target;
      if (event.target.files.length > 0) {
         let file = event.target.files[0];
         var fileReader = new FileReader();
         fileReader.addEventListener(
            'load',
            () => {
               var input = {
                  tipo: props.mimeType ? props.mimeType : file.type,
                  base64: fileReader.result,
                  descricao: file.name,
                  temporario: props.temporary,
               };
               api.post('/file', input).then((result) => {
                  setState({
                     descricao: result.descricao,
                  });
                  if (props.onChange) {
                     props.onChange(result);
                  }
                  tg.value = null;
               });
            },
            false
         );
         fileReader.readAsDataURL(file);
      }
   };

   const excluir = (event) => {
      var tg = event.target;
      showConfirm('Deseja realmente excluir o arquivo?', () => {
         if (props.onChange) {
            props.onChange(null);
         }
         setState({
            descricao: null,
         });
         tg.value = null;
      });
   };

   return (
      <div>
         <InputGroup>
            <Form.Control type='text' defaultValue={state.descricao} disabled={true} />
            <div style={{}} className='hide-when-readonly'>
               {!props.defaultValue ? (
                  <InputGroup.Text style={{ margin: '0', padding: '0' }}>
                     <Form.Label style={{ display: 'flex', height: 36, width: 48.5, cursor: 'pointer', margin: 0 }}>
                        <Form.File
                           accept={
                              props.accept
                                 ? props.accept
                                 : 'application/pdf, application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document'
                           }
                           onChange={onChange}
                           style={{ position: 'absolute', top: -1000 }}
                           tabIndex={-1}
                        />

                        <FontAwesomeIcon
                           style={{
                              fontSize: 20,
                              margin: 'auto',
                              color: LayoutParams.colors.corSecundaria,
                           }}
                           icon={faFolderOpen}
                        />
                     </Form.Label>
                  </InputGroup.Text>
               ) : (
                  <InputGroup.Text style={{ cursor: 'pointer', padding: 0 }}>
                     <div style={{ display: 'flex', flexDirection: 'row', height: '100%' }}>
                        <div
                           title={'Excluir arquivo'}
                           onClick={excluir}
                           style={{ display: 'flex', height: '100%', width: 40 }}
                        >
                           <div style={{ height: 24, width: 30, display: 'table-cell', paddingTop: 1, margin: 'auto' }}>
                              <IconButton
                                 style={{
                                    fontSize: 20,
                                    color: '#666',
                                    margin: 'auto',
                                 }}
                                 icon={faTrashAlt}
                              />
                           </div>
                        </div>
                        <div
                           title={'Baixar arquivo'}
                           onClick={baixar}
                           style={{
                              display: 'flex',
                              borderLeftStyle: 'solid',
                              borderLeftWidth: 1,
                              borderLeftColor: '#ced4da',
                              height: '100%',
                              width: 40,
                           }}
                        >
                           <div style={{ height: 24, width: 30, display: 'table-cell', paddingTop: 1, margin: 'auto' }}>
                              <IconButton
                                 style={{
                                    fontSize: 20,
                                    color: '#666',
                                 }}
                                 icon={faDownload}
                              />
                           </div>
                        </div>
                     </div>
                  </InputGroup.Text>
               )}
            </div>
         </InputGroup>
      </div>
   );
}
