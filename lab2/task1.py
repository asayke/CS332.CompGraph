import numpy as np
import cv2
from PIL import Image
import matplotlib.pyplot as plt

def rgb_to_gray(image, coefficients):
    # Преобразуем изображение в numpy-массив
    image_np = np.array(image)
    # Применяем формулу: Y = coef[0]*R + coef[1]*G + coef[2]*B
    gray_image = (coefficients[0] * image_np[:,:,0] +
                  coefficients[1] * image_np[:,:,1] +
                  coefficients[2] * image_np[:,:,2]).astype(np.uint8)
    return gray_image


image = Image.open('ugolovka2.jpg')
plt.imshow(image)
# Формула 1: Y = 0.299R + 0.587G + 0.114B
coefficients1 = [0.299, 0.587, 0.114]
gray_image1 = rgb_to_gray(image, coefficients1)

coefficients2 = [0.2126, 0.7152, 0.0722]
gray_image2 = rgb_to_gray(image, coefficients2)


difference_image = cv2.absdiff(gray_image1, gray_image2)

plt.figure(figsize=(12, 6))

plt.subplot(2, 3, 1)
plt.hist(gray_image1.ravel(), bins=256, range=(0, 256), color='gray')
plt.title('Histogram (Formula 1)')

plt.subplot(2, 3, 2)
plt.hist(gray_image2.ravel(), bins=256, range=(0, 256), color='gray')
plt.title('Histogram (Formula 2)')

plt.subplot(2, 3, 3)
plt.hist(difference_image.ravel(), bins=256, range=(0, 256), color='gray')
plt.title('Histogram (Difference)')

plt.subplot(2, 3, 4)
plt.imshow(gray_image1, cmap='gray')

plt.subplot(2, 3, 5)
plt.imshow(gray_image2, cmap='gray')

plt.subplot(2, 3, 6)
plt.imshow(difference_image, cmap='gray')

plt.tight_layout()
plt.show()

Image.fromarray(gray_image1).save('gray_image1.jpg')
Image.fromarray(gray_image2).save('gray_image2.jpg')
Image.fromarray(difference_image).save('difference_image.jpg')
