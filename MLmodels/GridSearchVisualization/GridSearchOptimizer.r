library(pracma)
library(data.table)
library(ggplot2)
library(gridExtra)
library(reshape2)

#source(file.path(dirname(dirname(rstudioapi::getActiveDocumentContext()$path)), "HelperScripts", "summary.r"))

### config ###

out.dir <- "generated-optimizer"

##############

base_working_dir <- dirname(rstudioapi::getActiveDocumentContext()$path)
setwd(base_working_dir)
data_dir <- file.path(base_working_dir, "data")

file <- 'input/output.csv'

data.raw <- read.csv(file, sep = ";")
data.raw$dropout = as.factor(data.raw$dropout)


### plot ###

data.plot = data.raw

data.plot = subset(data.plot, filter_size == 200 & dropout == 0.4)

#ggplot(data.plot, aes(x=filter_size, y=shape_output_accuracy, color=dropout, label=shape_output_accuracy_percentage)) + 
#  geom_point(size=5) + 
#  geom_text(nudge_x = 4) +
#  theme_minimal() +
#  geom_vline(xintercept = 64, linetype="dotted", color = "blue", size=1.5) +
#  scale_color_gradientn(colours = rainbow(10))

ids <- c("dropout","filter_size", "optimizer", "denses")

data.long <- melt(data.plot, id = ids, measure = c("test_shape_output_accuracy"))
#data.long <- melt(data.plot, id = c("dropout","filter_size"), measure = c("shape_output_accuracy", "size_output_accuracy"))
data.long$valuePercentage = paste(round(data.long$value * 100, 2),"%",sep="");


#data.long$filter_size = as.factor(data.long$filter_size)
lineSize <- 0.5


pShapeAccuracy <- ggplot(data.long, aes(x=factor(denses), y=value, fill=optimizer, label=valuePercentage)) + 
  geom_bar(stat='identity', position=position_dodge()) +
  geom_text(position = position_dodge(width = 1,)) +
  #geom_line() +
  #geom_vline(xintercept = 64, linetype="dotted", color = "red", size=lineSize) +
  theme_minimal()
#  scale_color_gradient(low = "black", high = "#999999")
#scale_color_gradient(low = 'greenyellow', high = 'forestgreen')

print(pShapeAccuracy)


data.longSize <- melt(data.plot, id = ids, measure = c("test_size_output_accuracy"))
data.longSize$valuePercentage = paste(round(data.longSize$value * 100, 2),"%",sep="");


#data.long$filter_size = as.factor(data.long$filter_size)

pSizeAccuracy <- ggplot(data.longSize, aes(x=factor(denses), y=value, fill=optimizer, label=valuePercentage)) + 
  geom_bar(stat='identity', position=position_dodge()) +
  geom_text(position = position_dodge(width = 1,)) +
  #  geom_line() +
  #geom_vline(xintercept = 64, linetype="dotted", color = "red", size=lineSize) +  
  theme_minimal()
#  scale_color_gradient(low = "black", high = "#999999")
#scale_color_gradient(low = 'greenyellow', high = 'forestgreen')



data.rotation <- melt(data.plot, id = ids, measure = c("test_rotation_output_angle_error"))
data.rotation$dropout = as.factor(data.rotation$dropout)
data.rotation$valueLabel = paste(round(data.rotation$value, 2),"°",sep="");

pRotationError <- ggplot(data.rotation, aes(x=factor(denses), y=value, fill=optimizer, label=valueLabel)) + 
  geom_bar(stat='identity', position=position_dodge()) +
  geom_text(position = position_dodge(width = 1,)) +
  #geom_vline(xintercept = 64, linetype="dotted", color = "red", size=lineSize) +
  #  geom_line() +
  theme_minimal()


data.loss <- melt(data.plot, id = ids, measure = c("test_shape_output_loss"))
#data.valueLabel = paste(round(data.rotation$value, 2),"°",sep="");

pShapeLoss <- ggplot(data.loss, aes(x=factor(denses), y=value, fill=optimizer, label=value)) + 
  geom_bar(stat='identity', position=position_dodge()) +
  geom_text(position = position_dodge(width = 1,)) +
  #geom_text(nudge_y = 1,nudge_x = 2, size=3) +
  #  geom_line() +
  geom_vline(xintercept = 64, linetype="dotted", color = "red", size=lineSize) +
  theme_minimal()

data.sizeloss <- melt(data.plot, id = ids, measure = c("test_size_output_loss"))
#data.valueLabel = paste(round(data.rotation$value, 2),"°",sep="");

pSizeLoss <- ggplot(data.sizeloss, aes(x=factor(denses), y=value, fill=optimizer, label=value)) + 
  geom_bar(stat='identity', position=position_dodge()) +
  geom_text(position = position_dodge(width = 1,)) +
  #geom_text(nudge_y = 1,nudge_x = 2, size=3) +
  #  geom_line() +
  geom_vline(xintercept = 64, linetype="dotted", color = "red", size=lineSize) +
  theme_minimal()

data.loss <- melt(data.plot, id = ids, measure = c("test_rotation_output_loss"))
#data.valueLabel = paste(round(data.rotation$value, 2),"°",sep="");

pRotationLoss <- ggplot(data.loss, aes(x=factor(denses), y=value, fill=optimizer, label=value)) + 
  geom_bar(stat='identity', position=position_dodge()) +
  geom_text(position = position_dodge(width = 1,)) +
  #geom_text(nudge_y = 1,nudge_x = 2, size=3) +
  # geom_line() +
  geom_vline(xintercept = 64, linetype="dotted", color = "red", size=lineSize) +
  theme_minimal()


#print(pShapeAccuracy)
#print(pSizeAccuracy)
#print(pShapeSizeLoss)
#print(pRotationError)
#print(pRotationLoss)

#pAll <- grid.arrange(pShapeSizeAccuracy, pShapeSizeLoss, pRotationError, pRotationLoss, nrow=4)

## save
width=10
height=5

gen.dir <- file.path(base_working_dir, out.dir)
dir.create(gen.dir,showWarnings = FALSE)


prefix = "pShapeAccuracy"
filename = paste(prefix, ".pdf", sep="")
outPath = file.path(gen.dir, filename)
print(outPath)
ggsave(outPath, plot=pShapeAccuracy, device = cairo_pdf, width = width, height = height)

prefix = "pSizeAccuracy"
filename = paste(prefix, ".pdf", sep="")
outPath = file.path(gen.dir, filename)
print(outPath)
ggsave(outPath, plot=pSizeAccuracy, device = cairo_pdf, width = width, height = height)


prefix = "pShapeLoss"
filename = paste(prefix, ".pdf", sep="")
outPath = file.path(gen.dir, filename)
print(outPath)
ggsave(outPath, plot=pShapeLoss, device = cairo_pdf, width = width, height = height)

prefix = "pShapeLoss"
filename = paste(prefix, ".pdf", sep="")
outPath = file.path(gen.dir, filename)
print(outPath)
ggsave(outPath, plot=pSizeLoss, device = cairo_pdf, width = width, height = height)


prefix = "pRotationError"
filename = paste(prefix, ".pdf", sep="")
outPath = file.path(gen.dir, filename)
print(outPath)
ggsave(outPath, plot=pRotationError, device = cairo_pdf, width = width, height = height)

prefix = "pRotationLoss"
filename = paste(prefix, ".pdf", sep="")
outPath = file.path(gen.dir, filename)
print(outPath)
ggsave(outPath, plot=pRotationLoss, device = cairo_pdf, width = width, height = height)

#prefix = "pAll"
#filename = paste(prefix, ".pdf", sep="")
#outPath = file.path(gen.dir, filename)
#print(outPath)
#ggsave(outPath, plot=pAll, device = cairo_pdf, width = width, height = height)
