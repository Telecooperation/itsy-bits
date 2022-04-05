rm(list=ls())
setwd(dirname(rstudioapi::getActiveDocumentContext()$path))

library(ggplot2)
library(gridExtra)
library(prismatic)
library(colorspace) # for plotting functions
library(magrittr) # for the glorious pipe



data.raw <- read.csv("stay_data.csv", sep=",", header=FALSE)



#folder <- "allShapes-abs"
#folder <- "figures"

#dir.create(folder, showWarnings = FALSE)

#data$Data <- as.character(data$Data)



data.clean = subset(data.raw, select = -c(V1,V2,V3) )
data.clean$id <- 1:nrow(data.clean)
data.clean[data.clean < 0] = 0
data.clean = data.clean[data.clean$id < 277,]

data.plot = data.clean

data.plot$mean <- rowMeans(data.clean, na.rm=TRUE)
data.plot$sum <- rowSums(data.clean, na.rm=TRUE)



#data.plot <- data.plot[data.plot$mean > 0,]

data.i1 = data.plot[data.plot$id >= 0 & data.plot$id <= 34,]
data.i2 = data.plot[data.plot$id >= 37 & data.plot$id <= 128,]
data.i3 = data.plot[data.plot$id >= 139 & data.plot$id <= 170,]
data.i4 = data.plot[data.plot$id >= 171 & data.plot$id <= 225,]
data.i5 = data.plot[data.plot$id >= 240 & data.plot$id <= 276,]

printMS <- function(name, data) {
  cat(paste("\t",name, ": ",round(mean(data),3), " (SD ", round(sd(data),3), ")\n", sep=""))
}


cat("For mean of pixels:\n")
printMS("nothing", data.i1$mean)
printMS("touched", data.i2$mean)
printMS("untouched", data.i3$mean)
printMS("touched", data.i4$mean)
printMS("untouched", data.i5$mean)

cat("For sum of pixels:\n")
printMS("nothing", data.i1$sum)
printMS("touched", data.i2$sum)
printMS("untouched", data.i3$sum)
printMS("touched", data.i4$sum)
printMS("untouched", data.i5$sum)


p <- ggplot(data.plot, aes(x = id, y = mean)) + 
  geom_point(size=1) +
  geom_line() +
  theme_minimal()
#  scale_color_gradient(low = "black", high = "#999999")
#scale_color_gradient(low = 'greenyellow', high = 'forestgreen')

print(p)





