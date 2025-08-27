import React, { useState, useMemo } from 'react';
import Form from '../components/forms/Form';
import CidadeController from './CidadeController';

export default function CidadeView(props) {
   const [state, setState] = useState({});

   // memoiza o controller para não recriar em cada render
   const controller = useMemo(
      () =>
         new CidadeController({
            get state() {
               return state;
            },
            setState,
            props,
         }),
      [state, props]
   );

   const getFormState = () => state;
   const setFormState = (newState, callback) => {
      return new Promise((resolve) => {
         setState((prev) => ({ ...prev, ...newState }));
         if (callback) callback();
         resolve();
      });
   };

   return (
      <Form
         titulo={'Cidades'}
         url={'/cidade'} 
         ordenacaoPadrao={'Id'}
         permissoes={[1111, null, null, null]}
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
