import { useState } from 'react';
import './../PreCadastro.css';

function Formulario({ formulario, setFormulario, mostrarTitulo }) {
   const [formData, setFormData] = useState(formulario ?? null);

   if (!formData) return null;

   const handleChange = (grupoIndex, campoIndex, valor) => {
      setFormData((prev) => {
         const novo = structuredClone(prev); // ou JSON.parse/stringify se não tiver polyfill
         novo.grupos[grupoIndex].campos[campoIndex].valor = valor;
         return novo;
      });
   };

   const handleSubmit = (e) => {
      e.preventDefault();
      console.log('Formulário: ', formData);
      setFormulario(formData);
   };

   return (
      <>
         <form onSubmit={handleSubmit} className='pre-cadastro-form'>
            {formData.grupos.map((grupo, grupoIndex) => (
               <div key={grupo.id} className='pre-cadastro-grupo'>
                  {grupo.titulo && mostrarTitulo && <span>{grupo.titulo}</span>}

                  {grupo.campos.map((campo, campoIndex) => {
                     const nomeDoCampo = `grupo-${grupo.id}-campo-${campo.id}`;

                     let result = null;

                     if (campo.tipo === 'texto') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group'>
                              <label htmlFor={nomeDoCampo}>{campo.titulo}</label>
                              <input
                                 type='text'
                                 id={nomeDoCampo}
                                 name={nomeDoCampo}
                                 value={formData.grupos[grupoIndex].campos[campoIndex].valor || ''}
                                 onChange={(e) => handleChange(grupoIndex, campoIndex, e.target.value)}
                                 required={campo.obrigatorio}
                              />
                           </div>
                        );
                     } else if (campo.tipo === 'simnao') {
                        result = (
                           <div key={campo.id} className='pre-cadastro-form-group'>
                              <label htmlFor={nomeDoCampo}>{campo.titulo}</label>
                              <select
                                 type='select'
                                 id={nomeDoCampo}
                                 name={nomeDoCampo}
                                 value={formData.grupos[grupoIndex].campos[campoIndex].valor || ''}
                                 onChange={(e) => handleChange(grupoIndex, campoIndex, e.target.value)}
                                 required={campo.obrigatorio}
                              >
                                 <option value=''></option>
                                 <option value='Sim'>Sim</option>
                                 <option value='Não'>Não</option>                                 
                              </select>
                           </div>
                        );
                     }

                     return result;
                  })}
               </div>
            ))}

            <button type='submit' className='pre-cadastro-submit-btn'>
               Enviar
            </button>
         </form>
      </>
   );
}

export default Formulario;
