import React, { useState, useMemo } from 'react';
import Form from './Form';
import PaisController from '../controllers/PaisController';

export default function PaisView(props) {
   const [state, setState] = useState({});

   // Cria o controller apenas uma vez (como no constructor da classe)
   const controller = useMemo(() => new PaisController({ state, setState, props }), []);

   // Métodos de integração com Form
   const getFormState = () => state;

   const setFormState = (newState, callback) => {
      return new Promise((resolve) => {
         setState((prev) => {
            const updated =
               typeof newState === 'function' ? newState(prev) : { ...prev, ...newState };
            return updated;
         });
         resolve();
         if (callback) callback();
      });
   };

   return (
      <Form
         titulo={'País'}
         url={'/pais'} 
         ordenacaoPadrao={'Id'}
         permissoes={[1331, null, null, null]}
         getFiltro={controller.getFiltro}
         getTitulosDaTabela={controller.getTitulosDaTabela}
         getDadosDaTabela={controller.getDadosDaTabela}
         renderizarFormulario={controller.renderizarFormulario}
         getObjetoDeDados={controller.getObjetoDeDados}
         antesDeInserir={controller.antesDeInserir}
         antesDeEditar={controller.antesDeEditar}
         antesDeSalvar={controller.antesDeSalvar}
         antesDeExcluir={controller.antesDeExcluir}
         getFormState={getFormState}
         setFormState={setFormState}
         select={props.select}
         itemVazio={{}}
      />
   );
}
