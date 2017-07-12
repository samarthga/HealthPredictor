#' @post /alexa
alexafunction <- function(Age,Sex,Education,BaselineAMD,Smoking,BMI,
                          AntioxidentGroup,ZincGroup)
  
{
  ExampleData <- read.table(file = 'ExampleData.txt')

  # specify dataset with outcome and predictor variables
  data(ExampleData)
  
  # rename column
  
  colnames(ExampleData)[2] <- "outcome"
  
  # fit logistic regression models
  riskmodelnew1 <- glm(outcome ~ Age + Sex + Education + 
                      BaselineAMD + Smoking + BMI + AntioxidentGroup + ZincGroup ,
                      family = binomial(link = "logit"), data = ExampleData)  
  
  #create df out of input function arguments
  arguments <- as.list(match.call())
  n = c(arguments$Age,arguments$Sex,arguments$Education,arguments$BaselineAMD,
        arguments$Smoking,arguments$BMI,arguments$AntioxidentGroup,arguments$ZincGroup)
  
  df <- data.frame(n) 
  
  # obtain predicted risks
  percentagerisk <- predict(riskmodelnew1,df, type = "response")
      return(percentagerisk)
  }
  
