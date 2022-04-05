rm(list=ls())
setwd(dirname(rstudioapi::getActiveDocumentContext()$path))

library("ggplot2")
library(prismatic)
library(colorspace) # for plotting functions
library(magrittr) # for the glorious pipe



data <- read.csv("raw/samples.csv", sep=",", header=FALSE)

names <- c("cross-tiny", "moon-big", "arrow-small", "heart-medium", "start-tiny", "star-small", "circle-tiny")


folder <- "samples"

dir.create(folder, showWarnings = FALSE)

#data$Data <- as.character(data$Data)



data = subset(data, select = -c(V1,V2) )

cap = data
data$Id <- 1:nrow(data)

#data$Mean <- rowMeans(data, na.rm=TRUE)


#data <- data[data$Mean > -50,]



#s <- strsplit(data$Data, split = ",")

#res <- data.frame(V1 = rep(data.measured$Id, sapply(s, length)), V2 = unlist(s))
#res$V2 <- as.numeric(as.character(res$V2))

#res <- res[res$V2 < 10000,]
#res <- res[res$V2 > 5,]


#mean(res$V2, na.rm=TRUE)


#ggplot(data = data, mapping = aes(x = Id, y = Mean)) + 
#  geom_point() +
#  geom_line()

#return
#m = matrix(runif(100),10,10)

x=20

#https://colorspace.r-forge.r-project.org/articles/hcl_palettes.html
palette="Grays"
#palette="YlOrRd"
#palette="Lajolla"  #image default
#palette="Dark Mint"
#palette="Cork"
#palette="Blues"
#palette="viridis"

#for (index in c(100:600)) {
for (index in c(1:nrow(cap))) {
  vec = as.numeric(cap[index,])
  m = matrix(vec,nrow = 15,ncol = 27)
  #m <- abs(m)
  #m <- m + 50;
  
  #print(min(m, na.rm = TRUE));
  
  png(filename=paste(folder,"/", names[index], ".png",sep=""), width=x*15, height=x*27, antialias = "none")
  m[m < 0] = 0
  par(mar=c(0, 0, 0, 0))
  #rev desaturate
  #image(m, useRaster=TRUE, axes=FALSE, col = (hcl.colors(280,palette)), zlim=c(-50,256))
  image(m, useRaster=TRUE, axes=FALSE, col = (hcl.colors(280,palette)))
  #image(m, useRaster=TRUE, axes=FALSE, zlim=c(-50,256))
  
  dev.off()
}

#m = matrix(runif(256),15,27)

palette="Grays"
index <- 3
vec = as.numeric(cap[index,])
m = matrix(vec,nrow = 15,ncol = 27)
m[m < 0] = 0
#image(m, useRaster=TRUE, axes=FALSE, col = (hcl.colors(280,palette)), zlim=c(-50,256))
image(m, useRaster=TRUE, axes=FALSE, col = (hcl.colors(280,palette)))

#par(mar=c(0, 0, 0, 0))
#image(m, useRaster=TRUE, axes=FALSE, col=grey.colors(255, start = 0, end = 1))

#library(grid)
#library(gridExtra)
#grid.arrange(p1, p2, ncol=1)

