import React, { useState, useMemo } from 'react';
import Form from '../components/forms/Form';
import ProfissaoController from './ProfissaoController';

export default function ProfissaoView(props) {
   const [state, setState] = useState({});

   // memoiza o controller para não recriar em cada render
   const controller = useMemo(() => new ProfissaoController(), [state]);

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
         titulo={'Profissões'}
         url={'/profissao'}
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
