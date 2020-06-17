// ����ͷ�ļ�
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "jfif.h"
#include "bmp.h"
#include "zigzag.h"
#include "jfif.h"
#include "stdefine.h"
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>


#include <time.h>
/*
const int ZIGZAG[64] =
{

     0,  1,  8, 16,  9,  2,  3, 10,

    17, 24, 32, 25, 18, 11,  4,  5,

    12, 19, 26, 33, 40, 48, 41, 34,

    27, 20, 13,  6,  7, 14, 21, 28,

    35, 42, 49, 56, 57, 50, 43, 36,

    29, 22, 15, 23, 30, 37, 44, 51,

    58, 59, 52, 45, 38, 31, 39, 46,

    53, 60, 61, 54, 47, 55, 62, 63,

};
void zigzag_decode_s(int *data)
{

    int buf[64], i;

  //soft

  for (i=0; i<64; i++) buf [ZIGZAG[i]] = data[i];
  for (i=0; i<64; i++) data[i] = buf[i];



    printf("from zigzag.c zigzag_decode ok\n");
//	close(zz_64_fd);

}

void zigzag_decode_h(int *data)
{

    int buf[64], i;
	  //int test[4]={0,10,20,30};
    int zz_64_fd = open("/dev/zz_4", O_RDWR);

    //int dct_value;

    if (zz_64_fd < 0)
    {

        printf("[ERROR] can't open de-zigzag ip");

    }

    for(i=0;i<64;i++){

	     write(zz_64_fd, &data[i], i);//0610

    }

    for( i=0;i<64;i++){
      read(zz_64_fd, &data[i], i);//0610
    }

    printf("from zigzag.c zigzag_decode ok\n");
	close(zz_64_fd);

}

void zigzag_decode_h2(int *data)
{
   int  i;
    for(i=0;i<64;i++){
	     write(zz_64_fd, &data[i], i);//0610
    }

    for( i=0;i<64;i++){
      read(zz_64_fd, &data[i], i);//0610
    }
}*/


int main(int argc, char *argv[])
{
    //timer define
    double dur;
    clock_t  start, end;
    int data[64];
    int min = 0;
    int max = 255;
    int i=0;
    //timer define ok

  //  void *jfif = NULL;
  //  BMP   bmp  = {0};

    if (argc < 3) {
        printf(
            "jfif test program\n"
            "usage: ffjpeg -s filename decode jpg file to decode.bmp\n"
            "       ffjpeg -h filename encode bmp file to encode.jpg\n"
        );
        return 0;
    }
    /* 產生 [min , max) 的浮點數亂數 */

     for( i=0;i<64;i++){
     	double x = (max - min) * rand() / (RAND_MAX + 1.0) + min;

     	data[i]=x;
     }
    //user start timer
    start = clock();
    //user start timer ok
    //jfif = jfif_load(argv[2]);
    //jfif_decode(jfif, &bmp);
   if (strcmp(argv[1], "-s") == 0) {
     for( i=0;i<1000;i++)zigzag_decode_s(data);
   } else if (strcmp(argv[1], "-h1") == 0) {
     for( i=0;i<1000;i++)zigzag_decode_h(data);
   }else if (strcmp(argv[1], "-h2") == 0) {
    zigzag_decode_h2(data);
    }

    //user stop timer
    end = clock();
    //user stop timer ok

    //user print time duration
    dur = (float)(end - start)/CLOCKS_PER_SEC;
    printf( "The process is done, time take *%f+ sec\n",dur);



    return 0;
}
