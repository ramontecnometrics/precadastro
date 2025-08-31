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

   const handleSubmit = () => {
      setFormulario(formData);
   };

   return (
      <>
         {formData.grupos.map((grupo, grupoIndex) => (
            <div key={grupo.id} className="pre-cadastro-grupo">
               {grupo.titulo && mostrarTitulo && <span>{grupo.titulo}</span>}

               {grupo.campos.map((campo, campoIndex) => {
                  const nomeDoCampo = `grupo-${grupo.id}-campo-${campo.id}`;
                  return (
                     <div key={campo.id} className="pre-cadastro-form-group">
                        <label htmlFor={nomeDoCampo}>{campo.titulo}</label>
                        <input
                           type="text"
                           id={nomeDoCampo}
                           name={nomeDoCampo}
                           value={formData.grupos[grupoIndex].campos[campoIndex].valor || ''}
                           onChange={(e) => handleChange(grupoIndex, campoIndex, e.target.value)}
                           required={campo.obrigatorio}
                        />
                     </div>
                  );
               })}
            </div>
         ))}

         <button type="submit" className="pre-cadastro-submit-btn" onClick={handleSubmit}>
            Enviar
         </button>
      </>
   );
}

export default Formulario;
