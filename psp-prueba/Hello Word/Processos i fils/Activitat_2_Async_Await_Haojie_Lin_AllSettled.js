function funcion1() {
    return new Promise(resolve => {
      setTimeout(() => {
        resolve('Promesa con 4 segundos de retraso');
      }, 4000);
    });
  }
  
  function funcion2() {
    return new Promise(resolve => {
      setTimeout(() => {
        resolve('Promesa con 5 segundos de retraso');
      }, 5000);
    });
  }
  
  function funcion3() {
    return new Promise(resolve => {
      setTimeout(() => {
        resolve('Promesa con 6 segundos de retraso');
      }, 6000);
    });
  }
  
  function funcion4() {
    return new Promise(resolve => {
      setTimeout(() => {
        resolve('Promesa con 7 segundos de retraso');
      }, 7000);
    });
  }
  
  function funcion5() {
    return new Promise(resolve => {
      setTimeout(() => {
        resolve('Promesa con 8 segundos de retraso');
      }, 8000);
    });
  }
  

  async function ejecutarSecuencialmente() {
    const resultados = [];
  
    resultados.push(await funcion1());
    console.log(resultados[resultados.length - 1]);
    resultados.push(await funcion2());
    console.log(resultados[resultados.length - 1]);
    resultados.push(await funcion3());
    console.log(resultados[resultados.length - 1]);
    resultados.push(await funcion4());
    console.log(resultados[resultados.length - 1]);
    resultados.push(await funcion5());
    console.log(resultados[resultados.length - 1]);
  
    return resultados;
  }
  
  async function ejecutarEnParalelo() {
    const promesas = [
      funcion1(),
      funcion2(),
      funcion3(),
      funcion4(),
      funcion5()
    ];
  
    const resultados = await Promise.allSettled(promesas);
  
    return resultados;
  }
  
  console.log('Ejecutando promesas secuencialmente:');
  ejecutarSecuencialmente().then(resultados => {
    console.log('\nEjecutando promesas en paralelo con Promise.allSettled:');
    ejecutarEnParalelo().then(resultadosParalelos => {
      console.log('Resultados en paralelo:', resultadosParalelos);
    });
  });