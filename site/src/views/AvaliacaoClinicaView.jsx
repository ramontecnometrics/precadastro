import React, { useState } from 'react';
import './../PreCadastro.css';
import Text from '../components/Text';
import api from '../utils/Api';
import { showError } from '../components/Messages';
import Formulario from './Formulario';

function AvaliacaoClinicaView({ idDoLead }) {
   const [formData, setFormData] = useState({});
   const [fichaPreenchida, setFichaPreenchida] = useState(false);
   const [idDoLeadLocal, setIdDoLeadLocal] = useState(idDoLead);
   const [cpf, setCpf] = useState();
   const [formulario, setFormulario] = useState(
      [
         {
            id: 1,
            titulo: null,
            campos: [
               {
                  id: 1,
                  titulo: 'A quanto tempo você começou a ter queda de cabelo?',
                  tipo: 'texto',
                  valor: null,
               },
            ],
         },
      ]
   );    

   const aplicarMascaraCPF = (valor) => {
      const apenasDigitos = valor.replace(/\D/g, '');
      return apenasDigitos.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
   };

   const handleChange = (e) => {
      const { name, value, type, checked, files } = e.target;

      let valorProcessado = value;

      setFormData((prevState) => ({
         ...prevState,
         [name]: type === 'checkbox' ? checked : type === 'file' ? files[0] : valorProcessado,
      }));
   };

   const handleSubmit = (e) => {
      e.preventDefault();
      console.log('Avaliacão:', formData);
      
   };

   const handleSubmitBuscarLead = (e) => {
      e.preventDefault();
      api.get(`/lead/identificar?cpf=${cpf}`)
         .then((response) => {
            setIdDoLeadLocal(response);
         })
         .catch((error) => {});
   };

   return (
      <div className='pre-cadastro'>
         <div className='pre-cadastro-container'>
            <header className='pre-cadastro-header'>
               <h1>Cadastro Dr. Hair</h1>
            </header>

            {!idDoLeadLocal && (
               <>
                  <main className='pre-cadastro-main-content'>
                     <form onSubmit={handleSubmitBuscarLead} className='pre-cadastro-form'>
                        <div className='pre-cadastro-form-section'>
                           <h2 className='pre-cadastro-section-title'>Avaliação clínica</h2>

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

            {idDoLeadLocal && !fichaPreenchida && (
               <>
                  <main className='pre-cadastro-main-content'>
                     <form onSubmit={handleSubmit} className='pre-cadastro-form'>
                        <div className='pre-cadastro-form-section'>
                           <h2 className='pre-cadastro-section-title'>Avaliação clínica</h2>
                           <Formulario setFormData={setFormData} formData={formData} formulario={formulario} />
                           <br/>
                        </div>
                        <button type='submit' className='pre-cadastro-submit-btn'>
                           Enviar
                        </button>
                     </form>
                  </main>
               </>
            )}

            {idDoLeadLocal && fichaPreenchida && (
               <>
                  <header className='pre-cadastro-header'>
                     <h1>Cadastro Dr. Hair</h1>
                     <br />
                     <p>Cadastro realizado com sucesso!</p>
                  </header>
               </>
            )}
         </div>
      </div>
   );
}

export default AvaliacaoClinicaView;
