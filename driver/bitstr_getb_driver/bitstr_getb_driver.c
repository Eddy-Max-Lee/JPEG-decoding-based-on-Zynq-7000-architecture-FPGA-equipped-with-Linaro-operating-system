#include <linux/init.h>
#include <linux/kernel.h>
#include <linux/module.h>
#include <linux/of_platform.h>
#include <linux/of_address.h>
#include <linux/miscdevice.h>
#include <linux/fs.h>
#include <linux/io.h>
#include <linux/uaccess.h>

MODULE_LICENSE("GPL");
#define DEVICE_NAME "bitstr_getb"
#define MYGPIO_PHY_ADDR 0x43C10000	// XPAR_MYGPIO_0_S00_AXI_BASEADDR -> 0x43C00000
#define MYGPIO_REG_NUM 4			// MYGPIO register number

void __iomem *GPIO_Regs;			// declare the address space is mapped from device address

/* "open" is the function to open device */
// Kernel use inode to point to file
// inode : the information of device opened by user process
static int call_bitstr_getb_open(struct inode *inode , struct file *filp)
{
	return 0;
}

/* "release" is the function to release device */
static int call_bitstr_getb_release(struct inode *inode , struct file *filp)
{
	return 0;
}

static ssize_t call_bitstr_getb_read(struct file *fp, char *buffer, size_t reg, loff_t *offset)
{
	unsigned int val = 0;

	if(reg >= 0 && reg < MYGPIO_REG_NUM)
	{
		val = ioread32(GPIO_Regs + reg * 4);
	}
	else
	{
		printk(KERN_ALERT"my_gpio : [ERROR] Read Wrong register number !\n");
	}
	
	copy_to_user(buffer, &val, 4);

	return 0;
}

static ssize_t call_bitstr_getb_write(struct file *fp, const char *buffer, size_t reg, loff_t *offset)
{
	unsigned int val = 0;

	copy_from_user(&val, buffer, 4);
	
	if(reg >= 0 && reg < MYGPIO_REG_NUM)
	{
		iowrite32(val, GPIO_Regs + reg * 4);
	}
	else
	{
		printk(KERN_ALERT"my_gpio : [ERROR] Write Wrong register number !\n");
	}
	return 0;
}

// file_operation data structure is the center of Linux character device, when program calls kernel module, all depend on the content in file_operation , defined in <linux/fs.h>
static const struct file_operations my_gpio_fops = 
{
	.owner   	= THIS_MODULE,			// To avoid the module to be unmounted when operating, so it always initialize to "THIS_MODULE"
	.open    	= call_bitstr_getb_open,
	.release 	= call_bitstr_getb_release,
	.read     	= call_bitstr_getb_read,
	.write		= call_bitstr_getb_write
};

static struct miscdevice my_gpio_dev =
{
	.minor = MISC_DYNAMIC_MINOR,		// minor-device number is secondary device, and MISC_DYNAMIC_MINOR means to be allocated by system automatically
	.name  = DEVICE_NAME,				// This name will show under /dev route
	.fops  = &my_gpio_fops
};

static int __init mygpio_init(void)
{
	printk(KERN_ALERT"Hello world bitstr_getb driver\n");
	GPIO_Regs = ioremap(MYGPIO_PHY_ADDR, MYGPIO_REG_NUM);	//ioremap(cookie, size)
	if(!GPIO_Regs)
		printk(KERN_ALERT"bitstr_getb: ioremap error\n");
	misc_register(&my_gpio_dev);
	return 0;
}

module_init(mygpio_init);