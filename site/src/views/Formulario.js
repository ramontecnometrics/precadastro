import { useEffect, useState } from 'react';
import './../PreCadastro.css';

function Formulario({ formData, setFormData, formulario }) {
   useEffect(() => {
      setFormData((prevState) => ({
         ...prevState,
         avaliacao: formulario,
      }));
   }, []);

   return !formData.avaliacao ? null : (
      <>
         {formulario.map((grupo, grupoIndex) => {
            var result = null;

            if (grupo.campos && grupo.campos.length > 0) {
               result = (
                  <>
                     {grupo.titulo && <span>{grupo.titulo}</span>}

                     {grupo.campos.map((campo, campoIndex) => {
                        return (
                           <div key={campoIndex} className='pre-cadastro-form-group'>
                              <label htmlFor={`campo-${campo.id}`}>{campo.titulo}</label>
                              <input
                                 type='text'
                                 id='nome'
                                 name='nome'
                                 value={formData.avaliacao[grupoIndex].campos[campoIndex].valor}
                                 onChange={(e) => {
                                    formData.avaliacao[grupoIndex].campos[campoIndex].valor = e.target.value;
                                 }}
                                 required={campo.obrigatorio}
                              />
                           </div>
                        );
                     })}
                  </>
               );
            }

            return result;
         })}
      </>
   );
}

export default Formulario;
