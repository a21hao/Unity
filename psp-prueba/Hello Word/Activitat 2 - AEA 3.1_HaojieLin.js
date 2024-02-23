const express = require('express');
const app = express();
const port = 3000;
const bodyParser = require('body-parser');

app.use(bodyParser.json());

const questions = require('./preguntas.json'); 

/*app.post('/getPreguntes', (req, res) => {
  const selectedQuestions = getRandomQuestions(req.query.num);
  //console.log(req.query);
  res.json(selectedQuestions);
});*/
app.post('/getPreguntes', (req, res) => {
  const selectedQuestions = getRandomQuestions(req.query.num);
  const sanitizedQuestions = selectedQuestions.map((question) => {
    const { correctIndex, ...questionWithoutCorrectIndex } = question;
    return questionWithoutCorrectIndex;
  });

  res.json(sanitizedQuestions);
});

app.post('/finalista', (req, res) => {
  const selectedAnswers = req.body;
  const correctAnswers = checkAnswers(selectedAnswers);
  res.json({ success: correctAnswers, total: selectedAnswers.length });
});

function getRandomQuestions(num) {
  const selectedQuestions = [];
  for (let i = 0; i < num; i++) {
    const randomIndex = Math.floor(Math.random() * questions.questions.length);
    selectedQuestions.push(questions.questions[randomIndex]);
  }
  return selectedQuestions;
}

function checkAnswers(selectedAnswers) {
  let correctCount = 0;
  for (let i = 0; i < selectedAnswers.length; i++) {
    const question = questions.questions[selectedAnswers[i].id];
    if (question && question.correctIndex === selectedAnswers[i].answerIndex) {
      correctCount++;
    }
  }
  return correctCount;
}

app.listen(port, () => {
  console.log(`La aplicación está escuchando en el puerto ${port}`);
});
