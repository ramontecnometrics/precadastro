import React, { useRef, useState } from 'react';
import './../PreCadastro.css';
import Text from '../components/Text';
import api from '../utils/Api';
import { showError, showInfo } from '../components/Messages';
import Formulario from './Formulario';

function AnamneseView() {
   const [fichaPreenchida, setFichaPreenchida] = useState(false);
   const [parametros, setParametros] = useState();
   const [cpf, setCpf] = useState();
   const containerRef = useRef(null);

   const aplicarMascaraCPF = (valor) => {
      const apenasDigitos = valor.replace(/\D/g, '');
      return apenasDigitos.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
   };

   const handleSubmitBuscarLead = (e) => {
      e.preventDefault();

      const params = new URL(location.href).searchParams;
      const id = params.get('id');

      api.get(`/lead/anamnese/parametros?cpf=${cpf}`)
         .then((response) => {
            if (response.idDoLead) {
               setParametros(response);
            } else {
               if (id) {
                  window.location = `./?id=${id}&cpf=${cpf}`;
               } else {
                  window.location = `./?cpf=${cpf}`;
               }
            }
         })
         .catch((error) => {});
   };

   const enviarAnamnese = (formulario) => {
      console.log('Anamnese:', formulario);

      let input = {
         idDoLead: parametros.idDoLead,
         fichaDeAnamnese: formulario,
      };

      api.post('/lead/precadastro/anamnese', input, null, null, {
         TokenParaAnamnese: parametros.tokenParaAnamnese,
      })
         .then((response) => {
            setFichaPreenchida(true);
         })
         .catch((error) => {});
   };

   return (
      <div className='pre-cadastro'>
         <div className='pre-cadastro-container' ref={containerRef}>
            {!parametros && (
               <>
                  <header className='pre-cadastro-header'>
                     <h1>Cadastro Dr. Hair</h1>
                  </header>

                  <main className='pre-cadastro-main-content'>
                     <form onSubmit={handleSubmitBuscarLead} className='pre-cadastro-form'>
                        <div className='pre-cadastro-form-section'>
                           <h2 className='pre-cadastro-section-title'>Anamnese</h2>

                           <div className='pre-cadastro-form-row'>
                              <div className='pre-cadastro-form-group'>
                                 <label htmlFor='cpf'>CPF</label>
                                 <input
                                    type='text'
                                    id='cpf'
                                    name='cpf'
                                    value={cpf}
                                    onChange={(e) => {
                                       if (e.target.value.length === 11) {
                                          setCpf(aplicarMascaraCPF(e.target.value));
                                       } else {
                                          setCpf(e.target.value);
                                       }
                                    }}
                                    placeholder='000.000.000-00'
                                    maxLength='14'
                                    required
                                 />
                              </div>
                              <div className='pre-cadastro-form-group' style={{ maxWidth: 80, marginTop: 14 }}>
                                 <button type='submit' className='pre-cadastro-submit-btn'>
                                    OK
                                 </button>
                              </div>
                           </div>
                        </div>
                     </form>
                  </main>
               </>
            )}

            {parametros && parametros.idDoLead && !fichaPreenchida && (
               <>
                  <header className='pre-cadastro-header'>
                     <h1>Cadastro Dr. Hair</h1>
                  </header>
                  <main className='pre-cadastro-main-content'>
                     <div className='pre-cadastro-form-section'>
                        <h2 className='pre-cadastro-section-title'>Anamnese</h2>
                        <Formulario
                           nome='anamnese'
                           formulario={parametros.fichaDeAnamnese}
                           setFormulario={enviarAnamnese}
                           mostrarTitulo={true}
                           containerRef={containerRef}
                        />
                        <br />
                     </div>
                  </main>
               </>
            )}

            {parametros && parametros.idDoLead && fichaPreenchida && (
               <>
                  <header className='pre-cadastro-header'>
                     <h1>Cadastro Dr. Hair</h1>
                     <br />
                     <p>Anamnese enviada com sucesso!</p>
                  </header>
               </>
            )}
         </div>
      </div>
   );
}

export default AnamneseView;
